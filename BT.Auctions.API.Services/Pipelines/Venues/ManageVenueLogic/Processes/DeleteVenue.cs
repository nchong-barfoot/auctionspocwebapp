using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes
{
    /// <summary>
    /// Process to Delete Venue from the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Venue}" />
    public class DeleteVenue : IProcess<Venue>
    {
        private readonly IVenueRepository _venueRepository;

        public DeleteVenue(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task Execute(Venue passObject)
        {
            await _venueRepository.DeleteVenue(passObject);
        }

        public string CancellationMessage { get; set; }
    }
}