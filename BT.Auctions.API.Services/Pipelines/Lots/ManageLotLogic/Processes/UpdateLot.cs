using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Process to Updarte Lot to the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Lot}" />
    public class UpdateLot : IProcess<Lot>
    {
        private readonly ILotRepository _lotRepository;

        public UpdateLot(ILotRepository auctionsRepository)
        {
            _lotRepository = auctionsRepository;
        }

        public async Task Execute(Lot passObject)
        {
            await _lotRepository.UpdateLot(passObject.LotId, passObject);
        }

        public string CancellationMessage { get; set; }
    }
}