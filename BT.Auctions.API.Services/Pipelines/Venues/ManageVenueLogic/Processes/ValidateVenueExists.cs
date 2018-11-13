using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes
{
    /// <summary>
    /// Process to Validate venue already exists. Deemed valid if a venue matching the id is returned from the db
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Venue}" />
    public class ValidateVenueExists : IProcess<Venue>
    {
        private readonly IVenueRepository _venueRepository;

        public ValidateVenueExists(IVenueRepository auctionsRepository)
        {
            _venueRepository = auctionsRepository;
        }

        public async Task Execute(Venue passObject)
        {
            try
            {
                await _venueRepository.GetVenueById(passObject.VenueId);
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operatio
                //n exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No Venue was found matching ID {passObject.VenueId}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}