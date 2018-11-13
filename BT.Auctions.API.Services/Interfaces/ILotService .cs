using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Services.Interfaces
{
    public interface ILotService
    {
        /// <summary>
        /// Adds the lot.
        /// </summary>
        /// <param name="lot">The lot.</param>
        /// <returns>The Added Lot</returns>
        Task<Lot> AddLot(LotDto lot);

        /// <summary>
        /// Gets the lot by identifier.
        /// </summary>
        /// <param name="auctionSessionId">The identifier.</param>
        /// <returns>Collection of Lots</returns>
        Task<IEnumerable<Lot>> GetLotsByAuctionSessionId(int auctionSessionId);

        /// <summary>
        /// Gets the lot by identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        Task<Lot> GetLotById(int lotId);

        /// <summary>
        /// Gets the lot details with bids by identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        Task<Lot> GetLotDetailsWithBidsById(int lotId);

        /// <summary>
        /// Gets the lots.
        /// </summary>
        /// <returns>Collection of Lots</returns>
        Task<IEnumerable<Lot>> GetLots();

        /// <summary>
        /// Updates the lot.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <param name="lot">The lot.</param>
        /// <returns>The Updated Lot</returns>
        Task<Lot> UpdateLot(int lotId, LotDto lot);
    }
}