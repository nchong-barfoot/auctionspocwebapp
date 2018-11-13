using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Process to Get AuctionSession from the Database
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{AuctionSession}" />
    public class UpdateAuctionSession : IProcess<AuctionSession>
    {
        private readonly IAuctionSessionRepository _auctionSessionRepository;

        public UpdateAuctionSession(IAuctionSessionRepository auctionSessionRepository)
        {
            _auctionSessionRepository = auctionSessionRepository;
        }

        public async Task Execute(AuctionSession passObject)
        {
            await _auctionSessionRepository.UpdateAuctionSession(passObject.AuctionSessionId, passObject);
        }

        public string CancellationMessage { get; set; }
    }
}