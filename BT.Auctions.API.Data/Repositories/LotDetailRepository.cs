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
    /// LotDetail repository to handle all database communications for lotDetail specific operations
    /// </summary>
    /// <seealso cref="T:BT.Auctions.API.Data.Repositories.ILotDetailRepository" />
    /// <seealso cref="T:BT.Auctions.API.Models.LotDetail" />
    public class LotDetailRepository : ILotDetailRepository
    {
        private readonly AuctionsContext _auctionsContext;

        public LotDetailRepository(AuctionsContext auctionsContext)
        {
            _auctionsContext = auctionsContext;
        }

        /// <summary>
        /// Adds the lotDetail.
        /// </summary>
        /// <param name="lotDetail">The lotDetail.</param>
        /// <returns>LotDetail</returns>
        public async Task<LotDetail> AddLotDetail(LotDetail lotDetail)
        {
            var addedLotDetail = _auctionsContext.LotDetail.AddAsync(lotDetail);
            await _auctionsContext.SaveChangesAsync();
            return addedLotDetail.Result.Entity;
        }

        /// <summary>
        /// Gets the lotDetail by identifier.
        /// </summary>
        /// <param name="lotDetailId">The identifier.</param>
        /// <returns>LotDetail</returns>
        public async Task<LotDetail> GetLotDetailById(int lotDetailId)
        {
            return await _auctionsContext.LotDetail.AsNoTracking()
                .SingleAsync(v => v.LotDetailId == lotDetailId);
        }

        /// <summary>
        /// Gets the lotDetail by lotDetail identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The requested LotDetail
        /// </returns>
        public async Task<LotDetail> GetLotDetailsByLotIdAndKey(int lotId, string key)
        {
            return await _auctionsContext.LotDetail.AsNoTracking()
                .Where(detail => detail.LotId == lotId)
                .FirstOrDefaultAsync(a => a.Key.Equals(key));
        }

        /// <summary>
        /// Gets the lotDetails by lot identifier.
        /// </summary>
        /// <param name="lotId">The LotDetail's Lot identifier.</param>
        /// <returns>LotDetails that belong to the Lot</returns>
        public async Task<IEnumerable<LotDetail>> GetLotDetailsByLotId(int lotId)
        {
            return await _auctionsContext.LotDetail
                .Include(l => l.Lot)
                .Where(l => l.LotId == lotId).ToListAsync();
        }

        /// <summary>
        /// Updates the lotDetail.
        /// </summary>
        /// <param name="lotDetailId">the id of the lotDetail to update</param>
        /// <param name="lotDetail">The lotDetail.</param>
        /// <returns>The Updated LotDetail</returns>
        public async Task<LotDetail> UpdateLotDetail(int lotDetailId, LotDetail lotDetail)
        {
            _auctionsContext.Entry(lotDetail).State = EntityState.Modified;
            await _auctionsContext.SaveChangesAsync();
            return lotDetail;
        }

        /// <summary>
        /// Deletes the lot detail.
        /// </summary>
        /// <param name="lotDetailId">The lot detail identifier.</param>
        /// <returns></returns>
        public async Task DeleteLotDetail(int lotDetailId)
        {
            _auctionsContext.LotDetail.Remove(
                _auctionsContext.LotDetail.Single(detail => detail.LotDetailId == lotDetailId));
            await _auctionsContext.SaveChangesAsync();
        }
    }
}
