using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes
{
    /// <summary>
    /// Process to Delete Display from the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Display}" />
    public class DeleteDisplay : IProcess<Display>
    {
        private readonly IDisplayRepository _displayRepository;

        public DeleteDisplay(IDisplayRepository displayRepository)
        {
            _displayRepository = displayRepository;
        }

        public async Task Execute(Display passObject)
        {
            await _displayRepository.DeleteDisplay(passObject.DisplayId);
        }

        public string CancellationMessage { get; set; }
    }
}