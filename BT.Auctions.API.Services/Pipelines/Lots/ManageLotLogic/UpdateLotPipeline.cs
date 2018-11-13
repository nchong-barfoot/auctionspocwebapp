using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic
{
    /// <summary>
    /// Update Lot Pipeline, 
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{Lot}" />
    public class UpdateLotPipeline : LogicPipeline<Lot>
    {
        public UpdateLotPipeline(ILogger<Lot> logger, ILotRepository lotRepository, IAuctionSessionRepository auctionSessionRepository) : base(logger)
        {
            Add(new ValidateLotHasAddress());
            Add(new ValidateLotSoldStatus());
            Add(new ValidateLotOrderIsUniqueInAuctionSession());
            Add(new ValidateLotHasUniqueListingIdInAuctionSession());
            Add(new ValidateLotAuctionSessionExists(auctionSessionRepository));
            Add(new UpdateLot(lotRepository));
        }
    }
}