using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.DisplayGroups.ManageDisplayGroupLogic.Processes
{
    /// <summary>
    /// Process to Add DisplayGroup to the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayGroup}" />
    public class AddDisplayGroup : IProcess<DisplayGroup>
    {
        private readonly IDisplayGroupRepository _displayRepository;

        public AddDisplayGroup(IDisplayGroupRepository displayRepository)
        {
            _displayRepository = displayRepository;
        }

        public async Task Execute(DisplayGroup passObject)
        {
            await _displayRepository.AddDisplayGroup(passObject);
        }

        public string CancellationMessage { get; set; }
    }
}