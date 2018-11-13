using System;
using System.Threading.Tasks;
using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;

namespace BT.Auctions.API.Services.Pipelines.DisplayConfigurations.ManageDisplayConfigurationLogic.Processes
{
    /// <summary>
    /// Process to Validate Venue exists for Display Group entity
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{DisplayConfiguration}" />
    public class ValidateDisplayGroupVenueExists : IProcess<DisplayGroup>
    {
        private readonly IVenueRepository _venueRepository;

        public ValidateDisplayGroupVenueExists(IVenueRepository venueRepository)
        {
            _venueRepository = venueRepository;
        }

        public async Task Execute(DisplayGroup passObject)
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