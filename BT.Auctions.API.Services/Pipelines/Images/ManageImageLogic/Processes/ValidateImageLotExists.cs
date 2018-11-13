using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using System;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Images.ManageImageLogic.Processes
{
    /// <summary>
    /// Process to Validate image lot exists
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Image}" />
    public class ValidateImageLotExists : IProcess<Image>
    {
        private readonly ILotRepository _lotRepository;

        public ValidateImageLotExists(ILotRepository lotRepository)
        {
            _lotRepository = lotRepository;
        }

        public async Task Execute(Image passObject)
        {
            try
            {
                await _lotRepository.GetLotById(passObject.LotId);
            }
            catch(InvalidOperationException)
            {
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"Lot {passObject.LotId} does not exist.";
            }
        }

        public string CancellationMessage { get; set; }
    }
}