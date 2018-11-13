using System;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Interfaces;
using BT.Auctions.API.Services.Pipelines.Bids.ManageBidLogic;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Services
{
    /// <summary>
    /// Bid Service to handle logic and pipeline execution for Bid endpoint
    /// </summary>
    /// <seealso cref="BT.Auctions.API.Services.Interfaces.IBidService" />
    public class BidService : IBidService
    {
        private readonly AddBidPipeline _addBidLogicPipeline;
        private readonly UpdateBidPipeline _updateBidLogicPipeline;

        private readonly IBidRepository _bidRepository;
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BidService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="bidRepository">The auctions repository.</param>
        /// <param name="lotRepository">The lot repository.</param>
        /// <param name="mapper">The mapper.</param>
        public BidService(ILogger<Bid> logger, IBidRepository bidRepository, ILotRepository lotRepository, IMapper mapper)
        {
            _bidRepository = bidRepository;
            _lotRepository = lotRepository;
            _mapper = mapper;
            _updateBidLogicPipeline = new UpdateBidPipeline(logger, _bidRepository, _lotRepository);
            _addBidLogicPipeline = new AddBidPipeline(logger, _bidRepository, _lotRepository);
        }

        /// <summary>
        /// Posts the bid.
        /// </summary>
        /// <param name="sourceBid">The bid.</param>
        /// <returns></returns>
        public async Task<Bid> AddBid(BidDto sourceBid)
        {
            //Listing ID to be validated before mapping and pipeline as int? will default to int and allow for a valid 0 result
            var mappedBid = _mapper.Map<BidDto, Bid>(sourceBid);
            return await _addBidLogicPipeline.Execute(mappedBid);
        }

        /// <summary>
        /// Gets the bids by lot identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Bid>> GetBidsByLotId(int lotId)
        {
            return await _bidRepository.GetBidsByLotId(lotId);
        }

        /// <summary>
        /// Gets the latest bids by lot identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Bid>> GetLatestBidsByLotId(int lotId)
        {
            return await _bidRepository.GetLatestBidsByLotId(lotId);
        }

        /// <summary>
        /// Updates the bid.
        /// </summary>
        /// <param name="bidId">The bid identifier.</param>
        /// <param name="sourceBid">The source bid.</param>
        /// <returns></returns>
        public async Task<Bid> UpdateBid(int bidId, BidDto sourceBid)
        {
            Bid mappedBid;
            try
            {
                mappedBid = _mapper.Map(sourceBid, await _bidRepository.GetBid(bidId));
            }
            catch (InvalidOperationException)
            {
                return await Task.FromResult(new Bid()
                {
                    CancellationReason = $"Could not find Bid {bidId} in System",
                    IsCancelled = true
                });
            }
            await _updateBidLogicPipeline.Execute(mappedBid);
            return await _bidRepository.GetBid(bidId);
        }
    }
}