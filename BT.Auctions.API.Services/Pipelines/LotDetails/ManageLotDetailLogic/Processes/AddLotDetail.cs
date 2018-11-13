using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.LotDetails.ManageLotDetailLogic.Processes
{
    /// <summary>
    /// Process to Add LotDetail to the Database. Note: this must be called after validation processes have been completed
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{LotDetail}" />
    public class AddLotDetail : IProcess<Models.LotDetail>
    {
        private readonly ILotDetailRepository _lotDetailRepository;

        public AddLotDetail(ILotDetailRepository auctionsRepository)
        {
            _lotDetailRepository = auctionsRepository;
        }

        public async Task Execute(Models.LotDetail passObject)
        {
            await _lotDetailRepository.AddLotDetail(passObject);
        }

        public string CancellationMessage { get; set; }
    }
}