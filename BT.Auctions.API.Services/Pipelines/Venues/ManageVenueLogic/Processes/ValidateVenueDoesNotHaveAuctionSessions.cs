using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes
{
    /// <summary>
    /// Process to Validate venue is apart of an Auction Session. Venues are no longer allowed to be deleted
    /// if they are associated to a session.
    /// Deemed valid if a venue matching the id is returned from the db
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Venue}" />
    public class ValidateVenueDoesNotHaveAuctionSessions : IProcess<Venue>
    {
        private readonly IAuctionSessionRepository _auctionSessionRepository;

        public ValidateVenueDoesNotHaveAuctionSessions(IAuctionSessionRepository auctionSessionRepository)
        {
            _auctionSessionRepository = auctionSessionRepository;
        }

        public async Task Execute(Venue passObject)
        {
            var sessionList = (await _auctionSessionRepository.GetAuctionSessionsByVenueId(passObject.VenueId)).ToList();
            if (sessionList.Any())
            {
                //Sessions exist. Send Cancellation
                passObject.IsCancelled = true;
                passObject.CancellationReason =
                    $"Cannot remove venue as there are {sessionList.Count} session(s) associated with it";
            }
        }

        public string CancellationMessage { get; set; }
    }
}