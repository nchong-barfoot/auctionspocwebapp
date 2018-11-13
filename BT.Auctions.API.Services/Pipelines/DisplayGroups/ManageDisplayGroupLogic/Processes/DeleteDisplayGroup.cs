using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Process to Delete DisplayGroup from the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayGroup}" />
    public class DeleteDisplayGroup : IProcess<DisplayGroup>
    {
        private readonly IDisplayGroupRepository _displayRepository;

        public DeleteDisplayGroup(IDisplayGroupRepository displayRepository)
        {
            _displayRepository = displayRepository;
        }

        public async Task Execute(DisplayGroup passObject)
        {
            await _displayRepository.DeleteDisplayGroup(passObject.DisplayGroupId);
        }

        public string CancellationMessage { get; set; }
    }
}