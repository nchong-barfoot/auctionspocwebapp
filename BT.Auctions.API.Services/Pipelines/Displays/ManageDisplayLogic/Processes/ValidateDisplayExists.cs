using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes
{
    /// <summary>
    /// Process to Validate display already exists. Deemed valid if a display matching the id is returned from the db
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Display}" />
    public class ValidateDisplayExists : IProcess<Display>
    {
        private readonly IDisplayRepository _displayRepository;

        public ValidateDisplayExists(IDisplayRepository displayRepository)
        {
            _displayRepository = displayRepository;
        }

        public async Task Execute(Display passObject)
        {
            try
            {
                await _displayRepository.GetDisplayById(passObject.DisplayId);
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No Display was found matching ID {passObject.DisplayId}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}