using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes
{
    /// <summary>
    /// Process to Validate Display configuration mapped Display exists in the system
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayConfiguration}" />
    public class ValidateDisplayConfigurationDisplayExists : IProcess<DisplayConfiguration>
    {
        private readonly IDisplayRepository _displayRepository;

        public ValidateDisplayConfigurationDisplayExists(IDisplayRepository displayRepository)
        {
            _displayRepository = displayRepository;
        }

        public async Task Execute(DisplayConfiguration passObject)
        {
            try
            {
                await _displayRepository.GetDisplayById(passObject.DisplayId);
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"Could not find Display {passObject.DisplayId} in the system. Please create the display before assigning it to a configuration";
            }
        }

        public string CancellationMessage { get; set; }
    }
}