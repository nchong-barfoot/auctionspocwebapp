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
    /// Lot repository to handle all database communications for lot specific operations
    /// </summary>
    /// <seealso cref="T:BT.Auctions.API.Data.Repositories.ILotRepository" />
    /// <seealso cref="T:BT.Auctions.API.Models.Lot" />
    public class LotRepository : ILotRepository
    {
        private readonly AuctionsContext _auctionsContext;

        public LotRepository(AuctionsContext auctionsContext)
        {
            _auctionsContext = auctionsContext;
        }

        /// <summary>
        /// Adds the lot.
        /// </summary>
        /// <param name="lot">The lot.</param>
        /// <returns>Lot</returns>
        public async Task<Lot> AddLot(Lot lot)
        {
            var addedLot = _auctionsContext.Lots.AddAsync(lot);
            await _auctionsContext.SaveChangesAsync();
            return addedLot.Result.Entity;
        }

        /// <summary>
        /// Gets the lot by identifier.
        /// </summary>
        /// <param name="lotId">The identifier.</param>
        /// <returns>Lot</returns>
        public async Task<Lot> GetLotById(int lotId)
        {
            return await _auctionsContext.Lots.AsNoTracking().SingleAsync(v => v.LotId == lotId);
        }

        /// <summary>
        /// Gets all the lot details including joined entities (including a subset of bids) by identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns>Lot</returns>
        public async Task<Lot> GetLotDetailsById(int lotId)
        {
            var lot = await _auctionsContext.Lots
                .Include(l => l.LotDetail)
                .Include(l => l.Images)
                .Include(l => l.Bids).AsNoTracking()
                .SingleAsync(v => v.LotId == lotId);

            lot.Bids = _auctionsContext.Bids.OrderByDescending(b => b.BidId)
                .Where(b => b.LotId == lot.LotId).AsNoTracking().Take(3).ToList();

            return lot;
        }

        /// <summary>
        /// Gets all the lot details including joined entities (including all bids) by identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns>Lot</returns>
        public async Task<Lot> GetLotDetailsWithBidsById(int lotId)
        {
            var lot = await _auctionsContext.Lots
                .Include(l => l.LotDetail).AsNoTracking()
                .Include(l => l.Images)
                .Include(l => l.Bids)
                .SingleAsync(v => v.LotId == lotId);

            return lot;
        }

        /// <summary>
        /// Gets the lots.
        /// </summary>
        /// <returns>List of Lots</returns>
        public async Task<IEnumerable<Lot>> GetLots()
        {
            var lots = _auctionsContext.Lots.AsNoTracking();
            return await lots.ToListAsync();
        }

        /// <summary>
        /// Gets the lots by auction session identifier.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Lot>> GetLotsByAuctionSessionId(int auctionSessionId)
        {
            var lots = _auctionsContext.Lots.AsNoTracking()
                .Include(l => l.Images).AsNoTracking()
                .Include(l => l.Bids).AsNoTracking()
                .Where(l => l.AuctionSessionId == auctionSessionId)
                .OrderBy(l => l.Order);

            foreach (var lot in lots)
            {
                lot.Bids = _auctionsContext.Bids.OrderByDescending(b => b.BidId)
                    .Where(b => b.LotId == lot.LotId && !b.IsRejected).Take(3).ToList();
            }

            return await lots.ToListAsync();
        }

        /// <summary>
        /// Updates the lot.
        /// </summary>
        /// <param name="lotId">the id of the lot to update</param>
        /// <param name="lot">The lot.</param>
        /// <returns></returns>
        public async Task<Lot> UpdateLot(int lotId, Lot lot)
        {
            var dbLot = _auctionsContext.Lots
                .Where(l => l.LotId == lotId)
                .Include(l => l.Images)
                .Include(l => l.LotDetail)
                .SingleOrDefault();

            if (dbLot != null)
            {
                // Update lot object
                _auctionsContext.Entry(dbLot).CurrentValues.SetValues(lot);

                foreach (var image in dbLot.Images.ToList())
                {
                    if (!lot.Images.Any(c => c.ImageId == image.ImageId))
                        _auctionsContext.Images.Remove(image);
                }
                var imagesToAdd = new List<Image>();
                foreach (var image in lot.Images)
                {
                    var imageInDb = dbLot.Images
                        .Where(c => c.ImageId == image.ImageId)
                        .SingleOrDefault();

                    if (imageInDb != null)
                        _auctionsContext.Entry(image).CurrentValues.SetValues(image);
                    else
                    {
                        image.LotId = lotId;
                        imagesToAdd.Add(image);
                    }
                }

                foreach (var image in imagesToAdd)
                {
                    dbLot.Images.Add(image);
                }

                foreach (var lotDetail in dbLot.LotDetail.ToList())
                {
                    if (!lot.LotDetail.Any(c => c.LotDetailId == lotDetail.LotDetailId || c.Key == lotDetail.Key))
                        _auctionsContext.LotDetail.Remove(lotDetail);
                }

                // Update and Insert
                var lotDetailsToAdd = new List<LotDetail>();
                foreach (var detail in lot.LotDetail)
                {
                    var lotDetail = dbLot.LotDetail
                        .Where(c => c.LotDetailId == detail.LotDetailId || c.Key == detail.Key)
                        .SingleOrDefault();

                    if (lotDetail != null && lotDetail.Value != detail.Value)
                    {
                        lotDetail.Value = detail.Value;
                        _auctionsContext.Entry(lotDetail).State = EntityState.Modified;
                    }
                    else if(lotDetail == null)
                    {
                        detail.LotId = lotId;
                        lotDetailsToAdd.Add(detail);
                    }
                }

                foreach (var detail in lotDetailsToAdd)
                {
                    dbLot.LotDetail.Add(detail);
                }
            }

            await _auctionsContext.SaveChangesAsync();
            return lot;
        }
    }
}
