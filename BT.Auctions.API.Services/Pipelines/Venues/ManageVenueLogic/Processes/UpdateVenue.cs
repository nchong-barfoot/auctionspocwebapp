using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes
{
    /// <summary>
    /// Process to Updarte Venue to the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Venue}" />
    public class UpdateVenue : IProcess<Venue>
    {
        private readonly IVenueRepository _venueRepository;

        public UpdateVenue(IVenueRepository auctionsRepository)
        {
            _venueRepository = auctionsRepository;
        }

        public async Task Execute(Venue passObject)
        {
            await _venueRepository.UpdateVenue(passObject.VenueId, passObject);
        }

        public string CancellationMessage { get; set; }
    }
}