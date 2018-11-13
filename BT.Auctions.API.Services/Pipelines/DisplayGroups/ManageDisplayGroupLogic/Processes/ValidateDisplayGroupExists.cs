using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Process to Validate display already exists. Deemed valid if a display matching the id is returned from the db
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayGroup}" />
    public class ValidateDisplayGroupExists : IProcess<DisplayGroup>
    {
        private readonly IDisplayGroupRepository _displayRepository;

        public ValidateDisplayGroupExists(IDisplayGroupRepository displayRepository)
        {
            _displayRepository = displayRepository;
        }

        public async Task Execute(DisplayGroup passObject)
        {
            try
            {
                await _displayRepository.GetDisplayGroupById(passObject.DisplayGroupId);
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No DisplayGroup was found matching ID {passObject.DisplayGroupId}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}