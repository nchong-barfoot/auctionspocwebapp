using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Process to Add Auction Session to the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{AuctionSession}" />
    public class AddAuctionSession : IProcess<AuctionSession>
    {
        private readonly IAuctionSessionRepository _auctionSessionRepository;

        public AddAuctionSession(IAuctionSessionRepository auctionSessionRepository)
        {
            _auctionSessionRepository = auctionSessionRepository;
        }

        public async Task Execute(AuctionSession passObject)
        {
            await _auctionSessionRepository.AddAuctionSession(passObject);
        }

        public string CancellationMessage { get; set; }
    }
}