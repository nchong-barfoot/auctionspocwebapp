using AutoMapper;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Services.Interfaces;
using BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services
{
    /// <summary>
    /// DisplayConfiguration Service to handle logic and pipeline execution for DisplayConfiguration endpoint
    /// </summary>
    /// <seealso cref="BT.Auctions.API.Services.Interfaces.IDisplayConfigurationService" />
    public class DisplayConfigurationService : IDisplayConfigurationService
    {
        private readonly AddDisplayConfigurationPipeline _addDisplayConfigurationLogicPipeline;
        private readonly UpdateDisplayConfigurationPipeline _updateDisplayConfigurationLogicPipeline;
        private readonly DeleteDisplayConfigurationPipeline _deleteDisplayConfigurationLogicPipeline;

        private readonly IDisplayConfigurationRepository _displayConfigurationRepository;
        private readonly IDisplayRepository _displayRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly IDisplayGroupRepository _displayGroupRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayConfigurationService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="displayConfigurationRepository">The auctions repository.</param>
        /// <param name="displayRepository">The display repository.</param>
        /// <param name="venueRepository">The venue repository.</param>
        /// <param name="displayGroupRepository">The display group repository.</param>
        /// <param name="mapper">The mapper.</param>
        public DisplayConfigurationService(ILogger<DisplayConfiguration> logger, IDisplayConfigurationRepository displayConfigurationRepository, IDisplayRepository displayRepository, IVenueRepository venueRepository, IDisplayGroupRepository displayGroupRepository, IMapper mapper)
        {
            _displayConfigurationRepository = displayConfigurationRepository;
            _displayRepository = displayRepository;
            _venueRepository = venueRepository;
            _displayGroupRepository = displayGroupRepository;
            _mapper = mapper;
            _updateDisplayConfigurationLogicPipeline = new UpdateDisplayConfigurationPipeline(logger, _displayConfigurationRepository, _displayRepository, _displayGroupRepository);
            _addDisplayConfigurationLogicPipeline = new AddDisplayConfigurationPipeline(logger, _displayConfigurationRepository, _displayRepository, _displayGroupRepository);
            _deleteDisplayConfigurationLogicPipeline = new DeleteDisplayConfigurationPipeline(logger, _displayConfigurationRepository);
        }

        /// <summary>
        /// Posts the displayConfiguration.
        /// </summary>
        /// <param name="sourceDisplayConfiguration">The displayConfiguration.</param>
        /// <returns></returns>
        public async Task<DisplayConfiguration> AddDisplayConfiguration(DisplayConfigurationDto sourceDisplayConfiguration)
        {
            //require a display ID before allowing a configuration to be created
            if(!sourceDisplayConfiguration.DisplayId.HasValue)
            {
                return await Task.FromResult(new DisplayConfiguration()
                {
                    CancellationReason = $"A display ID is required to create a display configuration",
                    IsCancelled = true
                });
            }
            var mappedDisplayConfiguration = _mapper.Map<DisplayConfigurationDto, DisplayConfiguration>(sourceDisplayConfiguration);
            return await _addDisplayConfigurationLogicPipeline.Execute(mappedDisplayConfiguration);
        }

        /// <summary>
        /// Gets the display configurations.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DisplayConfiguration>> GetDisplayConfigurations()
        {
            return await _displayConfigurationRepository.GetDisplayConfigurations();
        }

        /// <summary>
        /// Gets the display configurations by display configuration identifier.
        /// </summary>
        /// <param name="displayGroupId">The display group identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DisplayConfigurationDto>> GetDisplayGroupConfigurationsByDisplayGroupId(int displayGroupId)
        {
            var displayConfigurations = await _displayConfigurationRepository.GetDisplayGroupConfigurationsByDisplayGroupId(displayGroupId);

            return Mapper.Map<IEnumerable<DisplayConfigurationDto>>(displayConfigurations);
        }

        /// <summary>
        /// Updates the display configuration.
        /// </summary>
        /// <param name="displayConfigurationId">The display configuration identifier.</param>
        /// <param name="sourceDisplayConfiguration">The source displayConfiguration.</param>
        /// <returns></returns>
        public async Task<DisplayConfiguration> UpdateDisplayConfiguration(int displayConfigurationId, DisplayConfigurationDto sourceDisplayConfiguration)
        {
            try
            {
                var mappedDisplayConfiguration = _mapper.Map(sourceDisplayConfiguration, await _displayConfigurationRepository.GetDisplayConfigurationById(displayConfigurationId));
                return await _updateDisplayConfigurationLogicPipeline.Execute(mappedDisplayConfiguration);
            }
            catch (InvalidOperationException)
            {
                return await Task.FromResult(new DisplayConfiguration()
                {
                    CancellationReason = $"Could not find Display Configuration {displayConfigurationId} in System",
                    IsCancelled = true
                });
            }
        }

        /// <summary>
        /// Deletes the Display configuration.
        /// </summary>
        /// <param name="displayConfigurationId">The display configuration identifier.</param>
        /// <returns>DisplayGroup Object with cancellation reason if it applies</returns>
        public async Task<DisplayConfiguration> DeleteDisplayConfiguration(int displayConfigurationId)
        {
            return await _deleteDisplayConfigurationLogicPipeline.Execute(new DisplayConfiguration { DisplayConfigurationId = displayConfigurationId});
        }
    }
}