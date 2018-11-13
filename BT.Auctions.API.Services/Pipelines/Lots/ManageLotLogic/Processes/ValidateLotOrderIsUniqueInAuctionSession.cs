using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Process to Validate Auction Session order number is unique
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Lot}" />
    public class ValidateLotOrderIsUniqueInAuctionSession : IProcess<Lot>
    {
        public async Task Execute(Lot passObject)
        {
                if (passObject.AuctionSessionLots != null && passObject.AuctionSessionLots.Any(l => l.LotId != passObject.LotId && l.Order.Equals(passObject.Order)))
                {
                    passObject.IsCancelled = true;
                    passObject.CancellationReason = $"Order number {passObject.Order} is not unique against the Auction Session";
                }
            await Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}