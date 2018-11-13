using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic.Processes
{
    /// <summary>
    /// Process to Validate Venue exists for Display entity
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Display}" />
    public class ValidateDisplayVenueExists : IProcess<Display>
    {
        private readonly IVenueRepository _venueRepository;

        public ValidateDisplayVenueExists(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task Execute(Display passObject)
        {
            try
            {
                await _venueRepository.GetVenueById(passObject.VenueId);
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No Venue was found matching ID {passObject.VenueId}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}