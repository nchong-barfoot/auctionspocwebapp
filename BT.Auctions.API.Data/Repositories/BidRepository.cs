using System;
using BT.Auctions.API.Data.Contexts;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Data.Repositories
{
    /// <inheritdoc />
    /// <summary>
    /// Bid repository to handle all database communications for bid specific operations
    /// </summary>
    /// <seealso cref="T:BT.Auctions.API.Data.Repositories.IBidRepository" />
    /// <seealso cref="T:BT.Auctions.API.Models.Bid" />
    public class BidRepository : IBidRepository
    {
        private readonly AuctionsContext _auctionsContext;

        public BidRepository(AuctionsContext auctionsContext)
        {
            _auctionsContext = auctionsContext;
        }

        /// <summary>
        /// Adds the bid.
        /// </summary>
        /// <param name="bid">The bid.</param>
        /// <returns>Bid</returns>
        public async Task<Bid> AddBid(Bid bid)
        {
            bid.CreatedOn = DateTime.UtcNow;
            var addedBid = _auctionsContext.Bids.AddAsync(bid);
            await _auctionsContext.SaveChangesAsync();
            return addedBid.Result.Entity;
        }

        /// <summary>
        /// Gets the bid.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Bid> GetBid(int id)
        {
            return await _auctionsContext.Bids.SingleAsync(b => b.BidId == id);
        }

        /// <summary>
        /// Gets the bids by lot identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Bid>> GetBidsByLotId(int lotId)
        {
            return await _auctionsContext.Bids.AsNoTracking()
                .Where(l => l.LotId == lotId).ToListAsync();
        }

        /// <summary>
        /// Gets the latest bids by lot identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Bid>> GetLatestBidsByLotId(int lotId)
        {
            return await _auctionsContext.Bids.AsNoTracking()
                .Where(b => b.LotId == lotId && !b.IsRejected)
                .OrderByDescending(b => b.BidId)
                .Take(3)
                .ToListAsync();
        }

        /// <summary>
        /// Updates the bid.
        /// </summary>
        /// <param name="bidId">the id of the bid to update</param>
        /// <param name="bid">The bid.</param>
        /// <returns></returns>
        public async Task<Bid> UpdateBid(int bidId, Bid bid)
        {
            var updateBid = await _auctionsContext.Bids.SingleAsync(b => b.BidId == bidId);
            updateBid.IsRejected = bid.IsRejected;
            updateBid.ReserveMet = bid.ReserveMet;
            updateBid.ModifiedOn = DateTime.UtcNow;
            _auctionsContext.Entry(updateBid).State = EntityState.Modified;
            await _auctionsContext.SaveChangesAsync();
            return bid;
        }
    }
}
