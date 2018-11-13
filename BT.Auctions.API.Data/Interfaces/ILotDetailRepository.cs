using BT.Auctions.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Data.Interfaces
{
    /// <summary>
    /// LotDetail Repository Interface
    /// </summary>
    public interface ILotDetailRepository
    {
        Task<LotDetail> AddLotDetail(LotDetail lotDetail);
        Task<LotDetail> GetLotDetailById(int id);
        Task<LotDetail> GetLotDetailsByLotIdAndKey(int lotId, string key);
        Task<LotDetail> UpdateLotDetail(int id, LotDetail lotDetail);
        Task<IEnumerable<LotDetail>> GetLotDetailsByLotId(int lotId);
        Task DeleteLotDetail(int lotDetailId);
    }
}
