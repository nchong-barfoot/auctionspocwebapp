using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic
{
    /// <summary>
    /// Update AuctionSession Pipeline, 
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{AuctionSession}" />
    public class AddAuctionSessionPipeline : LogicPipeline<AuctionSession>
    {
        public AddAuctionSessionPipeline(ILogger<AuctionSession> logger, IAuctionSessionRepository auctionSessionRepository, IVenueRepository venueRepository, IDisplayGroupRepository displayGroupRepository) : base(logger)
        {
            Add(new ValidateAuctionSessionHasName());
            Add(new ValidateAuctionSessionStartDateBeforeFinishDate());
            Add(new ValidateAuctionSessionHasValidVenue(venueRepository));
            Add(new ValidateAuctionSessionDisplayGroups(displayGroupRepository));
            Add(new AddAuctionSession(auctionSessionRepository));
        }
    }
}