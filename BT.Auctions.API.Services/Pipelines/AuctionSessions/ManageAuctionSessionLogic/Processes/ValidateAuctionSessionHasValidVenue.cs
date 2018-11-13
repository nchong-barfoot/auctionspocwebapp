using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Process to Validate Auction Session Venue already exists (if specified).
    /// Deemed valid if a venue matching the id is returned from the db
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{AuctionSession}" />
    public class ValidateAuctionSessionHasValidVenue : IProcess<AuctionSession>
    {
        private readonly IVenueRepository _venueRepository;

        public ValidateAuctionSessionHasValidVenue(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task Execute(AuctionSession passObject)
        {
            try
            {
                if(passObject.VenueId.HasValue)
                    await _venueRepository.GetVenueById(passObject.VenueId.Value);
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No Venue was found matching ID {passObject.VenueId}. Cannot assign Auction Session to Venue.";
            }
        }

        public string CancellationMessage { get; set; }
    }
}