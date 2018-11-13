using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Process to Add Lot to the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Lot}" />
    public class AddLot : IProcess<Lot>
    {
        private readonly ILotRepository _lotRepository;

        public AddLot(ILotRepository lotRepository)
        {
            _lotRepository = lotRepository;
        }

        public async Task Execute(Lot passObject)
        {
            await _lotRepository.AddLot(passObject);
        }

        public string CancellationMessage { get; set; }
    }
}