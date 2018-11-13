using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;
using BT.Auctions.API.Data.Interfaces;

namespace BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic.Processes
{
    /// <summary>
    /// Process to Validate lot detail is unique
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{LotDetail}" />
    public class ValidateLotDetailKeyAndLotIsUnique : IProcess<LotDetail>
    {
        private readonly ILotDetailRepository _lotDetailRepository;

        public ValidateLotDetailKeyAndLotIsUnique(ILotDetailRepository lotDetailRepository)
        {
            _lotDetailRepository = lotDetailRepository;
        }

        public async Task Execute(LotDetail passObject)
        {
            if (passObject.LotId == 0 || string.IsNullOrEmpty(passObject.Key))
            {
                passObject.IsCancelled = true;
                passObject.CancellationReason = "Lot ID and Key are required";
                return;
            }

            var lotDetailFound = await _lotDetailRepository.GetLotDetailsByLotIdAndKey(passObject.LotId, passObject.Key);
            if (lotDetailFound != null && lotDetailFound.LotDetailId != passObject.LotDetailId)
            {
                //lot detail is already in database with the same lot ID and key
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"Lot {passObject.LotId} already has a property with key {passObject.Key}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}