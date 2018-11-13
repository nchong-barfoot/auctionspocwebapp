using BT.Auctions.API.Common.Interfaces;
using BT.Auctions.API.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Pipelines.Lots.ManageLotLogic.Processes
{
    /// <summary>
    /// Process to Validate Auction Session has unique Listing ID
    /// </summary>
    /// <seealso cref="Common.Interfaces.IProcess{Lot}" />
    public class ValidateLotHasUniqueListingIdInAuctionSession : IProcess<Lot>
    {
        public async Task Execute(Lot passObject)
        {
            if(passObject.AuctionSessionLots != null && passObject.AuctionSessionLots.Any(lot => lot.LotId != passObject.LotId && lot.ListingId == passObject.ListingId))
            {
                passObject.IsCancelled = true;
                passObject.CancellationReason = $"A listing ID must be unique to the auction session. Listing ID {passObject.ListingId} already exists";
            }
            await Task.FromResult(0);
        }

        public string CancellationMessage { get; set; }
    }
}