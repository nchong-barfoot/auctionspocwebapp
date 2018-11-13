using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic.Processes
{
    /// <summary>
    /// Process to validate the Auction Session has a valid name. Name is valid if not null or empty
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{AuctionSession}" />
    public class ValidateAuctionSessionHasName : IProcess<AuctionSession>
    {
        public Task Execute(AuctionSession passObject)
        {
            if (string.IsNullOrWhiteSpace(passObject.Name))
            {
                passObject.IsCancelled = true;
                passObject.CancellationReason = "Auction Session Name was not provided";
            }
            return Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}