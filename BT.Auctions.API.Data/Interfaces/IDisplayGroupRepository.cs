using BT.Auctions.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Data.Interfaces
{
    /// <summary>
    /// Display Group Repository Interface
    /// </summary>
    public interface IDisplayGroupRepository
    {
        Task<DisplayGroup> AddDisplayGroup(DisplayGroup displayConfiguration);
        Task<DisplayGroup> GetDisplayGroupById(int displayGroupId);
        Task<DisplayGroup> UpdateDisplayGroup(int displayGroupId, DisplayGroup displayConfiguration);
        Task<IEnumerable<DisplayGroup>> GetDisplayGroups();
        Task DeleteDisplayGroup(int displayGroupId);
        IQueryable<DisplayGroup> GetPagedDisplayGroups();
    }
}