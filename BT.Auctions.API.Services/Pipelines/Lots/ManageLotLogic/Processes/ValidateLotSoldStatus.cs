using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Process to Validate lot sold status
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Lot}" />
    public class ValidateLotSoldStatus : IProcess<Lot>
    {
        public Task Execute(Lot passObject)
        {
            if(passObject.AuctionStatus == AuctionStatus.Sold && (passObject.SoldPrice == null || passObject.SoldPrice == 0))
            {
                passObject.CancellationReason = "A Sold Price is required to set a Lot Status to Sold";
                passObject.IsCancelled = true;
            }

            else if(passObject.AuctionStatus == AuctionStatus.Sold && passObject.SoldDate == null)
            {
                passObject.CancellationReason = "A Sold Date is required to set a Lot Status to Sold";
                passObject.IsCancelled = true;
            }
            
            return Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}