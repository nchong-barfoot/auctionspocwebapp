using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic.Processes
{
    /// <summary>
    /// Process to Validate lot already exists
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Lot}" />
    public class ValidateLotDetailLotExists : IProcess<LotDetail>
    {
        private readonly ILotRepository _lotRepository;

        public ValidateLotDetailLotExists(ILotRepository lotRepository)
        {
            _lotRepository = lotRepository;
        }
        
        public async Task Execute(LotDetail passObject)
        {
            try
            {
                await _lotRepository.GetLotById(passObject.LotId);
            }
            catch (InvalidOperationException)
            {
                //gracefully catch an invalid operation exception caused by an invalid single item lookup
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"No Lot was found matching ID {passObject.LotId}";
            }
        }

        public string CancellationMessage { get; set; }
    }
}