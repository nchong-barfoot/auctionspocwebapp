using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Services.Interfaces
{
    public interface IBidService
    {
        /// <summary>
        /// Adds the bid.
        /// </summary>
        /// <param name="bid">The bid.</param>
        /// <returns>The Added Bid</returns>
        Task<Bid> AddBid(BidDto bid);

        /// <summary>
        /// Gets the bids by lot identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        Task<IEnumerable<Bid>> GetBidsByLotId(int lotId);

        /// <summary>
        /// Updates the bid.
        /// </summary>
        /// <param name="bidId">The bid identifier.</param>
        /// <param name="bid">The bid.</param>
        /// <returns>The Updated Bid</returns>
        Task<Bid> UpdateBid(int bidId, BidDto bid);

        /// <summary>
        /// Gets the latest bids by lot identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        Task<IEnumerable<Bid>> GetLatestBidsByLotId(int lotId);
    }
}