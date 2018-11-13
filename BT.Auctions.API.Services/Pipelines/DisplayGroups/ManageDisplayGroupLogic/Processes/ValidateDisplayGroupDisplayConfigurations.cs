using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Process to Validate display configurations have displays and already exists
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayGroup}" />
    public class ValidateDisplayGroupDisplayConfigurations : IProcess<DisplayGroup>
    {
        private readonly IDisplayConfigurationRepository _displayRepository;

        public ValidateDisplayGroupDisplayConfigurations(IDisplayConfigurationRepository displayRepository)
        {
            _displayRepository = displayRepository;
        }

        public async Task Execute(DisplayGroup passObject)
        {
            var tempDisplayGroupConfiguration = new DisplayGroupConfigurations();
            try
            {
                if(passObject.DisplayGroupConfigurations == null)
                {
                    return;
                }

                foreach (var displayGroupConfiguration in passObject.DisplayGroupConfigurations)
                {
                    tempDisplayGroupConfiguration = displayGroupConfiguration;
                    var configuration = await _displayRepository.GetDisplayConfigurationById(displayGroupConfiguration.DisplayConfigurationId);
                    if(configuration.Display == null)
                    {
                        passObject.IsCancelled = true;
                        passObject.CancellationReason = $"Display Configuration {displayGroupConfiguration.DisplayConfigurationId} " +
                            $"does not have a valid Display. Please add a display before assigning it to a group";
                    }
                }
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No Display Configuration was found matching ID {tempDisplayGroupConfiguration.DisplayConfigurationId}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}