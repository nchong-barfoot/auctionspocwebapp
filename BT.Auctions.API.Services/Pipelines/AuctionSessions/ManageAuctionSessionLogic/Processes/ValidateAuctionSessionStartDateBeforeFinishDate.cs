using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Process to Validate Auction Session Dates are in order
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{AuctionSession}" />
    public class ValidateAuctionSessionStartDateBeforeFinishDate : IProcess<AuctionSession>
    {
        public async Task Execute(AuctionSession passObject)
        {
            if (passObject.StartDate.HasValue && passObject.FinishDate.HasValue &&
                passObject.StartDate.Value >= passObject.FinishDate.Value)
            {
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"Auction Session start date ({passObject.StartDate.Value}) is after the finish date ({passObject.FinishDate.Value})";
            }

            if (passObject.FinishDate.HasValue && !passObject.StartDate.HasValue)
            {
                passObject.IsCancelled = true;
                passObject.CancellationReason =
                    "Auction Session must have a start date before a finish date can be added";
            }

            await Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}