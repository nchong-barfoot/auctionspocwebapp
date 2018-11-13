using System;
using System.Threading.Tasks;
using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;

namespace BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes
{
    /// <summary>
    /// Process to Validate DisplayGroup exists for Display Configuration entity
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayConfiguration}" />
    public class ValidateDisplayConfigurationDisplayGroupExists : IProcess<DisplayConfiguration>
    {
        private readonly IDisplayGroupRepository _displayGroupRepository;

        public ValidateDisplayConfigurationDisplayGroupExists(IDisplayGroupRepository displayGroupRepository)
        {
            _displayGroupRepository = displayGroupRepository;
        }

        public async Task Execute(DisplayConfiguration passObject)
        {
            var tempDisplayGroup = new DisplayGroupConfigurations();
            try
            {
                if (passObject.DisplayGroupConfigurations == null)
                    return;
                foreach(var group in passObject.DisplayGroupConfigurations)
                {
                    tempDisplayGroup = group;
                    await _displayGroupRepository.GetDisplayGroupById(group.DisplayGroupId);
                }
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No DisplayGroup was found matching ID {tempDisplayGroup.DisplayGroupId}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}