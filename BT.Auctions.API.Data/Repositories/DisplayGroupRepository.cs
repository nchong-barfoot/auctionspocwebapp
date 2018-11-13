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
    /// DisplayGroup repository to handle all database communications for display specific operations
    /// </summary>
    /// <seealso cref="T:BT.Auctions.API.Data.Repositories.IDisplayGroupRepository" />
    /// <seealso cref="T:BT.Auctions.API.Models.DisplayGroup" />
    public class DisplayGroupRepository : IDisplayGroupRepository
    {
        private readonly AuctionsContext _auctionsContext;

        public DisplayGroupRepository(AuctionsContext auctionsContext)
        {
            _auctionsContext = auctionsContext;
        }

        private void PopulateDisplayGroup(DisplayGroup display)
        {
            //https://stackoverflow.com/questions/27176014/how-to-add-update-child-entities-when-updating-a-parent-entity-in-ef/27177623#27177623
            if (display.AuctionSessions != null)
            {
                var auctionSessionIds = display.AuctionSessions.Select(a => a.AuctionSessionId);
                display.AuctionSessions = _auctionsContext.AuctionSessions.Where(a => auctionSessionIds.Contains(a.AuctionSessionId)).ToList();
            }

            if (display.DisplayGroupConfigurations != null)
            {
                var displayConfigurationIds = display.DisplayGroupConfigurations.Select(d => d.DisplayConfigurationId);
                var displayConfigurationList = _auctionsContext.DisplayConfigurations.Where(d => displayConfigurationIds
                .Contains(d.DisplayConfigurationId)).Select(d => new DisplayGroupConfigurations
                {
                    DisplayConfiguration = d,
                    DisplayConfigurationId = d.DisplayConfigurationId
                }).ToList();
                display.DisplayGroupConfigurations = displayConfigurationList;
            }
        }

        /// <summary>
        /// Adds the display group.
        /// </summary>
        /// <param name="displayGroup">The display group to be added.</param>
        /// <returns>DisplayGroup</returns>
        public async Task<DisplayGroup> AddDisplayGroup(DisplayGroup displayGroup)
        {
            PopulateDisplayGroup(displayGroup);
            var addedDisplayGroup = _auctionsContext.DisplayGroups.AddAsync(displayGroup);
            await _auctionsContext.SaveChangesAsync();
            return addedDisplayGroup.Result.Entity;
        }

        /// <summary>
        /// Gets the display group by identifier.
        /// </summary>
        /// <param name="displayGroupId">The identifier.</param>
        /// <returns>DisplayGroup</returns>
        public async Task<DisplayGroup> GetDisplayGroupById(int displayGroupId)
        {
            return await _auctionsContext.DisplayGroups.AsNoTracking()
                .Include(d => d.DisplayGroupConfigurations)
                .ThenInclude(d => d.DisplayConfiguration).AsNoTracking()
                .Include(d => d.AuctionSessions).AsNoTracking()
                .SingleAsync(v => v.DisplayGroupId == displayGroupId);
        }

        /// <summary>
        /// Gets all displays.
        /// </summary>
        /// <returns>List of DisplayGroups</returns>
        public async Task<IEnumerable<DisplayGroup>> GetDisplayGroups()
        {
            return await _auctionsContext.DisplayGroups.AsNoTracking()
                .Include(d => d.DisplayGroupConfigurations)
                .ThenInclude(d => d.DisplayConfiguration).AsNoTracking()
                .Include(d => d.AuctionSessions).AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Gets the paged display groups.
        /// </summary>
        /// <returns></returns>
        public IQueryable<DisplayGroup> GetPagedDisplayGroups()
        {
            return _auctionsContext.DisplayGroups.AsNoTracking()
                .Include(d => d.DisplayGroupConfigurations)
                .ThenInclude(d => d.DisplayConfiguration)
                .ThenInclude(d => d.Display).AsNoTracking()
                .Where(d => d.DisplayGroupConfigurations != null)
                .AsQueryable();
        }

        /// <summary>
        /// Updates the display.
        /// </summary>
        /// <param name="displayGroupId">the id of the display group to update</param>
        /// <param name="display">The display.</param>
        /// <returns>The Updated DisplayGroup</returns>
        public async Task<DisplayGroup> UpdateDisplayGroup(int displayGroupId, DisplayGroup displayGroup)
        {
            PopulateDisplayGroup(displayGroup);
            _auctionsContext.Entry(displayGroup).State = EntityState.Modified;
            await _auctionsContext.SaveChangesAsync();
            return displayGroup;
        }


        /// <summary>
        /// Deletes the display group
        /// </summary>
        /// <param name="displayGroupId">The display group identifier.</param>
        /// <returns>void task</returns>
        public async Task DeleteDisplayGroup(int displayGroupId)
        {
            var displayToDelete = _auctionsContext.DisplayGroups
                .Include(d => d.AuctionSessions)
                .Include(d => d.DisplayGroupConfigurations)
                    .ThenInclude(d => d.DisplayConfiguration)
                    .ThenInclude(d => d.Display)
                .Include(d => d.DisplayGroupConfigurations)
                    .ThenInclude(d => d.DisplayGroup)
                .Single(d => d.DisplayGroupId == displayGroupId);
            _auctionsContext.Entry(displayToDelete).State = EntityState.Deleted;
            await _auctionsContext.SaveChangesAsync();
        }
    }
}
