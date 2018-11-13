using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes
{
    /// <summary>
    /// Process to Update DisplayConfiguration to the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayConfiguration}" />
    public class UpdateDisplayConfiguration : IProcess<DisplayConfiguration>
    {
        private readonly IDisplayConfigurationRepository _displayConfigurationRepository;

        public UpdateDisplayConfiguration(IDisplayConfigurationRepository displayConfigurationRepository)
        {
            _displayConfigurationRepository = displayConfigurationRepository;
        }

        public async Task Execute(DisplayConfiguration passObject)
        {
            await _displayConfigurationRepository.UpdateDisplayConfiguration(passObject.DisplayConfigurationId, passObject);
        }

        public string CancellationMessage { get; set; }
    }
}