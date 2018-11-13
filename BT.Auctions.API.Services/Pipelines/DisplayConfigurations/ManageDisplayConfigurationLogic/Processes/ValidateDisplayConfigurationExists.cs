using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes
{
    /// <summary>
    /// Process to Validate configuration already exists. Deemed valid if a display matching the id is returned from the db
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayConfiguration}" />
    public class ValidateDisplayConfigurationExists : IProcess<DisplayConfiguration>
    {
        private readonly IDisplayConfigurationRepository _displayConfigurationRepository;

        public ValidateDisplayConfigurationExists(IDisplayConfigurationRepository displayConfigurationRepository)
        {
            _displayConfigurationRepository = displayConfigurationRepository;
        }
        
        public async Task Execute(DisplayConfiguration passObject)
        {
            try
            {
                await _displayConfigurationRepository.GetDisplayConfigurationById(passObject.DisplayConfigurationId);
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No DisplayConfiguration was found matching ID {passObject.DisplayConfigurationId}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}