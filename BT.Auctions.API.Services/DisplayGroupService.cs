using AutoMapper;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Pagination;
using BT.Auctions.API.Services.Interfaces;
using BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services
{
    /// <summary>
    /// DisplayGroup Service to handle logic and pipeline execution for DisplayGroup endpoint
    /// </summary>
    /// <seealso cref="BT.Auctions.API.Services.Interfaces.IDisplayGroupService" />
    public class DisplayGroupService : IDisplayGroupService
    {
        private readonly AddDisplayGroupPipeline _addDisplayGroupLogicPipeline;
        private readonly UpdateDisplayGroupPipeline _updateDisplayGroupLogicPipeline;
        private readonly DeleteDisplayGroupPipeline _deleteDisplayGroupLogicPipeline;

        private readonly IDisplayGroupRepository _displayGroupRepository;
        private readonly IDisplayConfigurationRepository _displayConfigurationRepository;
        private readonly IAuctionSessionRepository _auctionSessionRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DisplayGroup> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayGroupService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="displayGroupRepository">The displayGroup repository.</param>
        /// <param name="displayConfigurationRepository">The display configuration repository.</param>
        /// <param name="auctionSessionRepository">The auction session repository.</param>
        /// <param name="venueRepository">The venue repository.</param>
        /// <param name="mapper">The mapper.</param>
        public DisplayGroupService(ILogger<DisplayGroup> logger, IDisplayGroupRepository displayGroupRepository, IDisplayConfigurationRepository displayConfigurationRepository, IAuctionSessionRepository auctionSessionRepository, IVenueRepository venueRepository, IMapper mapper)
        {
            _displayGroupRepository = displayGroupRepository;
            _displayConfigurationRepository = displayConfigurationRepository;
            _auctionSessionRepository = auctionSessionRepository;
            _venueRepository = venueRepository;
            _mapper = mapper;
            _logger = logger;
            _updateDisplayGroupLogicPipeline = new UpdateDisplayGroupPipeline(logger, _displayGroupRepository, _displayConfigurationRepository, _auctionSessionRepository, _venueRepository);
            _addDisplayGroupLogicPipeline = new AddDisplayGroupPipeline(logger, _displayGroupRepository, _displayConfigurationRepository, _auctionSessionRepository, _venueRepository);
            _deleteDisplayGroupLogicPipeline = new DeleteDisplayGroupPipeline(logger, _displayGroupRepository);
        }

        /// <summary>
        /// Posts the displayGroup.
        /// </summary>
        /// <param name="sourceDisplayGroup">The displayGroup.</param>
        /// <returns>the added displayGroup</returns>
        public async Task<DisplayGroup> AddDisplayGroup(DisplayGroupDto sourceDisplayGroup)
        {
            var mappedDisplayGroup = _mapper.Map<DisplayGroupDto, DisplayGroup>(sourceDisplayGroup);
            return await _addDisplayGroupLogicPipeline.Execute(mappedDisplayGroup);
        }

        /// <summary>
        /// Deletes the displayGroup.
        /// </summary>
        /// <param name="displayGroupId">The displayGroup identifier.</param>
        /// <returns>DisplayGroup Object with cancellation reason if it applies</returns>
        public async Task<DisplayGroup> DeleteDisplayGroup(int displayGroupId)
        {
            return await _deleteDisplayGroupLogicPipeline.Execute(new DisplayGroup { DisplayGroupId = displayGroupId });
        }

        /// <summary>
        /// Gets the display groups
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DisplayGroup>> GetDisplayGroups()
        {
            return await _displayGroupRepository.GetDisplayGroups();
        }

        /// <summary>
        /// Gets the paged display groups.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="venueId">The venue identifier.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns></returns>
        public async Task<PagedList<DisplayGroupDto>> GetPagedDisplayGroups(int pageNumber, int pageSize, int? auctionSessionId, int? venueId, string timeZone)
        {
            try
            {
                var auctionSessions = (await _auctionSessionRepository.GetAuctionSessionsOccuringOnDate(DateTimeOffset.UtcNow, timeZone));

                //either evaluate based on a passed in auction session date range or use the current date time
                if (auctionSessionId.HasValue)
                {
                    var selectedSession = auctionSessions.Single(a => a.AuctionSessionId == auctionSessionId);
                    auctionSessions = auctionSessions.Where(a =>
                        a.IsInSession &&
                        a.AuctionSessionId != selectedSession.AuctionSessionId &&
                         (!a.FinishDate.HasValue || a.FinishDate.Value > DateTime.UtcNow));
                }

                var displayGroupsInUse = auctionSessions.Where(a => a.DisplayGroupId.HasValue)
                    .Select(a => a.DisplayGroup).Distinct();

                var groupsInUse = displayGroupsInUse.ToList();
                var displayIdsInUse = groupsInUse
                    .SelectMany(groups => groups.DisplayGroupConfigurations)
                    .Select(groupConfigs => groupConfigs.DisplayConfiguration.DisplayId).Distinct().ToList();

                var query = _displayGroupRepository.GetPagedDisplayGroups();

                if (venueId.HasValue)
                {
                    query = query.Where(d => d.VenueId == venueId);
                }
                
                var totalItems = query.Count();
                var displayGroups = query
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToList();

                //get list of display ids in group
                //check if display ids are in the display groups that are in use
                displayGroups.ForEach(displayGroup =>
                    displayGroup.IsInUse = groupsInUse.Any(groupInUse =>
                        groupInUse.DisplayGroupId == displayGroup.DisplayGroupId || displayGroup.DisplayGroupConfigurations.Any(groupConfigurations =>
                            displayIdsInUse.Contains(groupConfigurations.DisplayConfiguration.DisplayId))));

                var mappedDisplayGroups = _mapper.Map<IEnumerable<DisplayGroupDto>>(displayGroups);
                var pagedResult = new PagedList<DisplayGroupDto>(mappedDisplayGroups, totalItems, pageNumber, pageSize);

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());
                return await Task.FromResult(new PagedList<DisplayGroupDto>
                {
                    CancellationReason = "Invalid parameters provided",
                    IsCancelled = true
                });
            }
        }

        /// <summary>
        /// Updates the displayGroup configuration.
        /// </summary>
        /// <param name="displayGroupId">The displayGroup configuration identifier.</param>
        /// <param name="sourceDisplayGroup">The source displayGroup.</param>
        /// <returns></returns>
        public async Task<DisplayGroup> UpdateDisplayGroup(int displayGroupId, DisplayGroupDto sourceDisplayGroup)
        {
            try
            {
                var mappedDisplayGroup = _mapper.Map(sourceDisplayGroup, await _displayGroupRepository.GetDisplayGroupById(displayGroupId));
                return await _updateDisplayGroupLogicPipeline.Execute(mappedDisplayGroup);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.ToString());
                return await Task.FromResult(new DisplayGroup()
                {
                    CancellationReason = $"Could not find DisplayGroup {displayGroupId} in System",
                    IsCancelled = true
                });
            }
        }
    }
}