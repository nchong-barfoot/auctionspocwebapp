using BT.Auctions.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Data.Interfaces
{
    /// <summary>
    /// Lot Repository Interface
    /// </summary>
    public interface ILotRepository
    {
        Task<Lot> AddLot(Lot lot);
        Task<Lot> GetLotById(int id);
        Task<Lot> GetLotDetailsById(int id);
        Task<IEnumerable<Lot>> GetLots();
        Task<Lot> UpdateLot(int id, Lot lot);
        Task<IEnumerable<Lot>> GetLotsByAuctionSessionId(int lotId);
        Task<Lot> GetLotDetailsWithBidsById(int lotId);
    }
}
