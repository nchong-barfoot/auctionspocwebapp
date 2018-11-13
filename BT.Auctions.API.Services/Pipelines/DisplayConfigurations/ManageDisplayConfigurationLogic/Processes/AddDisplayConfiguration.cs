using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes
{
    /// <summary>
    /// Process to Add Display Configuration to the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayConfiguration}" />
    public class AddDisplayConfiguration : IProcess<DisplayConfiguration>
    {
        private readonly IDisplayConfigurationRepository _displayConfigurationRepository;

        public AddDisplayConfiguration(IDisplayConfigurationRepository displayConfigurationRepository)
        {
            _displayConfigurationRepository = displayConfigurationRepository;
        }

        public async Task Execute(DisplayConfiguration passObject)
        {
            await _displayConfigurationRepository.AddDisplayConfiguration(passObject);
        }

        public string CancellationMessage { get; set; }
    }
}