using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic
{
    /// <summary>
    /// Delete Venue Pipeline. Has the default process flow constructed here
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{Venue}" />
    public class DeleteVenuePipeline : LogicPipeline<Venue>
    {
        public DeleteVenuePipeline(ILogger<Venue> logger, IVenueRepository venueRepository, IAuctionSessionRepository auctionSessionRepository) : base(logger)
        {
            Add(new ValidateVenueExists(venueRepository));
            Add(new ValidateVenueDoesNotHaveAuctionSessions(auctionSessionRepository));
            Add(new DeleteVenue(venueRepository));
        }
    }
}