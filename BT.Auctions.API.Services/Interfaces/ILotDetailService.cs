using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Services.Interfaces
{
    public interface ILotDetailService
    {
        /// <summary>
        /// Adds the lotDetail.
        /// </summary>
        /// <param name="lotDetail">The lotDetail.</param>
        /// <returns>The Added LotDetail</returns>
        Task<LotDetail> AddLotDetail(LotDetailDto lotDetail);

        /// <summary>
        /// Gets the lotDetail by identifier.
        /// </summary>
        /// <param name="lotId">The identifier.</param>
        /// <returns>Collection of LotDetails</returns>
        Task<IEnumerable<LotDetail>> GetLotDetailByLotId(int lotId);

        /// <summary>
        /// Updates the lotDetail.
        /// </summary>
        /// <param name="lotDetailId">The lotDetail identifier.</param>
        /// <param name="lotDetail">The lotDetail.</param>
        /// <returns>The Updated LotDetail</returns>
        Task<LotDetail> UpdateLotDetail(int lotDetailId, LotDetailDto lotDetail);

        /// <summary>
        /// Deletes the lot detail.
        /// </summary>
        /// <param name="lotDetailId">The lot detail identifier.</param>
        /// <returns></returns>
        Task<LotDetail> DeleteLotDetail(int lotDetailId);
    }
}