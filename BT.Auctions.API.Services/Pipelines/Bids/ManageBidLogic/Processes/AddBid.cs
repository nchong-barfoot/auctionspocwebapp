using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Bids.ManageBidLogic.Processes
{
    /// <summary>
    /// Process to Add Bid to the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Bid}" />
    public class AddBid : IProcess<Bid>
    {
        private readonly IBidRepository _bidRepository;

        public AddBid(IBidRepository bidRepository)
        {
            _bidRepository = bidRepository;
        }

        public async Task Execute(Bid passObject)
        {
            await _bidRepository.AddBid(passObject);
        }

        public string CancellationMessage { get; set; }
    }
}