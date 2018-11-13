using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Pagination;

namespace BT.Auctions.API.Services.Interfaces
{
    public interface IDisplayGroupService
    {
        /// <summary>
        /// Adds the lot.
        /// </summary>
        /// <param name="display">The DisplayGroup.</param>
        /// <returns>The Added DisplayGroup</returns>
        Task<DisplayGroup> AddDisplayGroup(DisplayGroupDto displayDto);

        /// <summary>
        /// Gets the display groups
        /// </summary>
        /// <returns>Collection of DisplayGroups</returns>
        Task<IEnumerable<DisplayGroup>> GetDisplayGroups();

        /// <summary>
        /// Updates the DisplayGroup.
        /// </summary>
        /// <param name="displayId">The display configuration identifier.</param>
        /// <param name="displayDto">The display configuration dto.</param>
        /// <returns>
        /// The Updated DisplayGroup
        /// </returns>
        Task<DisplayGroup> UpdateDisplayGroup(int displayId, DisplayGroupDto displayDto);

        /// <summary>
        /// Deletes the display.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<DisplayGroup> DeleteDisplayGroup(int id);

        /// <summary>
        /// Gets the paged display groups.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="venueId">The venue identifier.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns></returns>
        Task<PagedList<DisplayGroupDto>> GetPagedDisplayGroups(int pageNumber, int pageSize,
            int? auctionSessionId, int? venueId, string timeZone);
    }
}