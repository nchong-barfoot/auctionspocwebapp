using BT.Auctions.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Data.Interfaces
{
    /// <summary>
    /// Bid Repository Interface
    /// </summary>
    public interface IBidRepository
    {
        Task<Bid> AddBid(Bid bid);
        Task<Bid> UpdateBid(int id, Bid bid);
        Task<Bid> GetBid(int id);
        Task<IEnumerable<Bid>> GetBidsByLotId(int lotId);
        Task<IEnumerable<Bid>> GetLatestBidsByLotId(int lotId);
    }
}
