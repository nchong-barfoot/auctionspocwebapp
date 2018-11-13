using BT.Auctions.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Data.Interfaces
{
    /// <summary>
    /// Display Repository Interface
    /// </summary>
    public interface IDisplayRepository
    {
        Task<Display> AddDisplay(Display display);
        Task<Display> GetDisplayById(int id);
        Task<Display> UpdateDisplay(int id, Display display);
        Task<IEnumerable<Display>> GetDisplaysByVenueId(int displayId);
        Task DeleteDisplay(int displayId);
        Task<IEnumerable<Display>> GetDisplays();
    }
}