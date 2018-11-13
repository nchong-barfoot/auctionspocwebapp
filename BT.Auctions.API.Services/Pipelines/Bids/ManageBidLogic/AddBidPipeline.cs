using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.Bids.ManageBidLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.Bids.ManageBidLogic
{
    /// <summary>
    /// Add Bid Pipeline. Process pipeline used to validate and add a bid into the system
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{Bid}" />
    public class AddBidPipeline : LogicPipeline<Bid>
    {
        public AddBidPipeline(ILogger<Bid> logger, IBidRepository bidRepository, ILotRepository lotRepository) : base(logger)
        {
            Add(new ValidateBidHasAmount());
            Add(new ValidateBidLotExists(lotRepository));
            Add(new AddBid(bidRepository));
        }
    }
}