using AutoMapper;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Services.Interfaces;
using BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services
{
    /// <summary>
    /// LotDetail Service to handle logic and pipeline execution for LotDetail endpoint
    /// </summary>
    /// <seealso cref="BT.Auctions.API.Services.Interfaces.ILotDetailService" />
    public class LotDetailService : ILotDetailService
    {
        private readonly AddLotDetailPipeline _addLotDetailLogicPipeline;
        private readonly UpdateLotDetailPipeline _updateLotDetailLogicPipeline;

        private readonly ILotDetailRepository _lotDetailRepository;
        private readonly ILotRepository _lotRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LotDetailService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="lotDetailRepository">The auctions repository.</param>
        /// <param name="lotRepository">The lot repository.</param>
        /// <param name="mapper">The mapper.</param>
        public LotDetailService(ILogger<LotDetail> logger, ILotDetailRepository lotDetailRepository, ILotRepository lotRepository, IMapper mapper)
        {
            _lotDetailRepository = lotDetailRepository;
            _lotRepository = lotRepository;
            _mapper = mapper;
            _updateLotDetailLogicPipeline = new UpdateLotDetailPipeline(logger, _lotDetailRepository, _lotRepository);
            _addLotDetailLogicPipeline = new AddLotDetailPipeline(logger, _lotDetailRepository, _lotRepository);
        }

        /// <summary>
        /// Posts the lotDetail.
        /// </summary>
        /// <param name="sourceLotDetail">The lotDetail.</param>
        /// <returns></returns>
        public async Task<LotDetail> AddLotDetail(LotDetailDto sourceLotDetail)
        {
            var mappedLotDetail = _mapper.Map<LotDetailDto, LotDetail>(sourceLotDetail);
            return await _addLotDetailLogicPipeline.Execute(mappedLotDetail);
        }

        /// <summary>
        /// Gets the lotDetail by auction session identifier.
        /// </summary>
        /// <param name="auctionSessionId">The identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<LotDetail>> GetLotDetailByLotId(int lotId)
        {
            return await _lotDetailRepository.GetLotDetailsByLotId(lotId);
        }

        /// <summary>
        /// Updates the lotDetail.
        /// </summary>
        /// <param name="lotDetailId">The lotDetail identifier.</param>
        /// <param name="sourceLotDetail">The source lotDetail.</param>
        /// <returns></returns>
        public async Task<LotDetail> UpdateLotDetail(int lotDetailId, LotDetailDto sourceLotDetail)
        {
            try
            {
                var mappedLotDetail = _mapper.Map(sourceLotDetail, await _lotDetailRepository.GetLotDetailById(lotDetailId));
                return await _updateLotDetailLogicPipeline.Execute(mappedLotDetail);
            }
            catch (InvalidOperationException)
            {
                return await Task.FromResult(new LotDetail()
                {
                    CancellationReason = $"Could not find LotDetail {lotDetailId} in System",
                    IsCancelled = true
                });
            }
        }

        /// <summary>
        /// Deletes the lot detail.
        /// </summary>
        /// <param name="lotDetailId">The lot detail identifier.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<LotDetail> DeleteLotDetail(int lotDetailId)
        {
            try
            {
                await _lotDetailRepository.DeleteLotDetail(lotDetailId);
                return new LotDetail();
            }
            catch (InvalidOperationException)
            {
                return await Task.FromResult(new LotDetail()
                {
                    CancellationReason = $"Could not find LotDetail {lotDetailId} in System",
                    IsCancelled = true
                });
            }
        }
    }
}