using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Bids.ManageBidLogic.Processes
{
    /// <summary>
    /// Process to Validate bid amount
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Bid}" />
    public class ValidateBidHasAmount : IProcess<Bid>
    {
        public Task Execute(Bid passObject)
        {
            //bids require an amount
            if (passObject.Amount == 0)
            {
                passObject.CancellationReason = "Bid amount is required";
                passObject.IsCancelled = true;
            }
            return Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}