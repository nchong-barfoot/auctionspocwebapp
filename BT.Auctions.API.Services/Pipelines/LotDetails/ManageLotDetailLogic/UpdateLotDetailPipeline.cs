using BT.Auctions.API.Common.LogicPipelines;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic.Processes;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic
{
    /// <summary>
    /// Update LotDetail Pipeline, 
    /// </summary>
    /// <seealso cref="Common.LogicPipelines.LogicPipeline{LotDetail}" />
    public class UpdateLotDetailPipeline : LogicPipeline<LotDetail>
    {
        public UpdateLotDetailPipeline(ILogger<LotDetail> logger, ILotDetailRepository lotDetailRepository, ILotRepository lotRepository) : base(logger)
        {
            Add(new ValidateLotDetailKeyAndLotIsUnique(lotDetailRepository));
            Add(new ValidateLotDetailLotExists(lotRepository));
            Add(new UpdateLotDetail(lotDetailRepository));
        }
    }
}