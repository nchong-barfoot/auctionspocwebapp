using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic
{
    /// <summary>
    /// Update Venue Pipeline, 
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{Venue}" />
    public class UpdateVenuePipeline : LogicPipeline<Venue>
    {
        public UpdateVenuePipeline(ILogger<Venue> logger, IVenueRepository venueRepository) : base(logger)
        {
            Add(new ValidateVenueExists(venueRepository));
            Add(new UpdateVenue(venueRepository));
        }
    }
}