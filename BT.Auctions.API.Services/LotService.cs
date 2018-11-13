using System;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Interfaces;
using BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Services
{
    /// <summary>
    /// Lot Service to handle logic and pipeline execution for Lot endpoint
    /// </summary>
    /// <seealso cref="BT.Auctions.API.Services.Interfaces.ILotService" />
    public class LotService : ILotService
    {
        private readonly AddLotPipeline _addLotLogicPipeline;
        private readonly UpdateLotPipeline _updateLotLogicPipeline;

        private readonly ILotRepository _lotRepository;
        private readonly IAuctionSessionRepository _auctionSessionRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LotService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="lotRepository">The auctions repository.</param>
        /// <param name="auctionSessionRepository">The auction session repository.</param>
        /// <param name="lotDetailRepository">The lotDetail repository.</param>
        /// <param name="mapper">The mapper.</param>
        public LotService(ILogger<Lot> logger, ILotRepository lotRepository, IAuctionSessionRepository auctionSessionRepository, ILotDetailRepository lotDetailRepository, IMapper mapper)
        {
            _lotRepository = lotRepository;
            _auctionSessionRepository = auctionSessionRepository;
            _mapper = mapper;
            _updateLotLogicPipeline = new UpdateLotPipeline(logger, _lotRepository, _auctionSessionRepository);
            _addLotLogicPipeline = new AddLotPipeline(logger, _lotRepository, _auctionSessionRepository);
        }

        /// <summary>
        /// Posts the lot.
        /// </summary>
        /// <param name="sourceLot">The lot.</param>
        /// <returns></returns>
        public async Task<Lot> AddLot(LotDto sourceLot)
        {
            //Listing ID to be validated before mapping and pipeline as int? will default to int and allow for a valid 0 result
            if(!sourceLot.ListingId.HasValue)
                return await Task.FromResult(new Lot()
                {
                    CancellationReason = $"A Listing ID is required",
                    IsCancelled = true
                });
            var mappedLot = _mapper.Map<LotDto, Lot>(sourceLot);
            mappedLot.AuctionSessionLots = await _lotRepository.GetLotsByAuctionSessionId(mappedLot.AuctionSessionId);
            return await _addLotLogicPipeline.Execute(mappedLot);
        }

        /// <summary>
        /// Gets the lot by auction session identifier.
        /// </summary>
        /// <param name="auctionSessionId">The identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Lot>> GetLotsByAuctionSessionId(int auctionSessionId)
        {
            return await _lotRepository.GetLotsByAuctionSessionId(auctionSessionId);
        }

        /// <summary>
        /// Gets the lot details with bids by identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        public async Task<Lot> GetLotDetailsWithBidsById(int lotId)
        {
            return await _lotRepository.GetLotDetailsWithBidsById(lotId);
        }

        /// <summary>
        /// Gets the lot by identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        public async Task<Lot> GetLotById(int lotId)
        {
            return await _lotRepository.GetLotById(lotId);
        }

        /// <summary>
        /// Gets the lots.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Lot>> GetLots()
        {
            return await _lotRepository.GetLots();
        }

        /// <summary>
        /// Updates the lot.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <param name="sourceLot">The source lot.</param>
        /// <returns></returns>
        public async Task<Lot> UpdateLot(int lotId, LotDto sourceLot)
        {
            Lot mappedLot;
            try
            {
                mappedLot = _mapper.Map(sourceLot, await _lotRepository.GetLotDetailsWithBidsById(lotId));
                mappedLot.AuctionSessionLots = await _lotRepository.GetLotsByAuctionSessionId(mappedLot.AuctionSessionId);

                if (mappedLot.AuctionStatus == AuctionStatus.Sold && (mappedLot.SoldPrice == null || mappedLot.SoldPrice == 0))
                {
                    var latestBid = mappedLot.Bids.Where(b => !b.IsRejected)
                        .OrderByDescending(b => b.BidId).FirstOrDefault();

                    if (latestBid != null)
                    {
                        mappedLot.SoldPrice = latestBid.Amount;
                        mappedLot.SoldDate = DateTime.UtcNow;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                return await Task.FromResult(new Lot()
                {
                    CancellationReason = $"Could not find Lot {lotId} in System",
                    IsCancelled = true
                });
            }

            var lot = await _updateLotLogicPipeline.Execute(mappedLot);
            if (lot.IsCancelled)
                return lot;
            return await _lotRepository.GetLotDetailsWithBidsById(lotId);
        }
    }
}