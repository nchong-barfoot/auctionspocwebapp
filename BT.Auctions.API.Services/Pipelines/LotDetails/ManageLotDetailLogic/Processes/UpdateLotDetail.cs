using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic.Processes
{
    /// <summary>
    /// Process to Update LotDetail to the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{LotDetail}" />
    public class UpdateLotDetail : IProcess<Models.LotDetail>
    {
        private readonly ILotDetailRepository _lotDetailRepository;

        public UpdateLotDetail(ILotDetailRepository auctionsRepository)
        {
            _lotDetailRepository = auctionsRepository;
        }

        public async Task Execute(Models.LotDetail passObject)
        {
            await _lotDetailRepository.UpdateLotDetail(passObject.LotDetailId, passObject);
        }

        public string CancellationMessage { get; set; }
    }
}