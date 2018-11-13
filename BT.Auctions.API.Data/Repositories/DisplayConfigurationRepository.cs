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
    /// DisplayConfiguration repository to handle all database communications for displayConfiguration specific operations
    /// </summary>
    /// <seealso cref="T:BT.Auctions.API.Data.Repositories.IDisplayConfigurationRepository" />
    /// <seealso cref="T:BT.Auctions.API.Models.DisplayConfiguration" />
    public class DisplayConfigurationRepository : IDisplayConfigurationRepository
    {
        private readonly AuctionsContext _auctionsContext;

        public DisplayConfigurationRepository(AuctionsContext auctionsContext)
        {
            _auctionsContext = auctionsContext;
        }

        private void PopulateDisplayConfiguration(DisplayConfiguration displayConfiguration)
        {
            //https://stackoverflow.com/questions/27176014/how-to-add-update-child-entities-when-updating-a-parent-entity-in-ef/27177623#27177623
            if (displayConfiguration.DisplayGroupConfigurations != null)
            {
                var displayGroupIds = displayConfiguration.DisplayGroupConfigurations.Select(d => d.DisplayGroupId);
                var displayGroupList = _auctionsContext.DisplayGroups.Include(d => d.DisplayGroupConfigurations)
                    .ThenInclude(d => d.DisplayConfiguration).Where(d => displayGroupIds
                .Contains(d.DisplayGroupId)).Select(d => new DisplayGroupConfigurations
                {
                    DisplayGroup = d,
                    DisplayGroupId = d.DisplayGroupId
                }).ToList();
                displayConfiguration.DisplayGroupConfigurations = displayGroupList;
            }
        }

        /// <summary>
        /// Adds the displayConfiguration.
        /// </summary>
        /// <param name="displayConfiguration">The displayConfiguration.</param>
        /// <returns>DisplayConfiguration</returns>
        public async Task<DisplayConfiguration> AddDisplayConfiguration(DisplayConfiguration displayConfiguration)
        {
            PopulateDisplayConfiguration(displayConfiguration);
            var addedDisplayConfiguration = _auctionsContext.DisplayConfigurations.AddAsync(displayConfiguration);
            await _auctionsContext.SaveChangesAsync();
            return addedDisplayConfiguration.Result.Entity;
        }

        /// <summary>
        /// Gets the displayConfiguration by identifier.
        /// </summary>
        /// <param name="displayConfigurationId">The identifier.</param>
        /// <returns>DisplayConfiguration</returns>
        public async Task<DisplayConfiguration> GetDisplayConfigurationById(int displayConfigurationId)
        {
            return await _auctionsContext.DisplayConfigurations.Include(d => d.Display).AsNoTracking()
                .SingleAsync(d => d.DisplayConfigurationId == displayConfigurationId);
        }

        /// <summary>
        /// Gets the display configuration details by identifier.
        /// </summary>
        /// <param name="displayConfigurationId">The display configuration identifier.</param>
        /// <returns></returns>
        public async Task<DisplayConfiguration> GetDisplayConfigurationDetailsById(int displayConfigurationId)
        {
            return await _auctionsContext.DisplayConfigurations.Include(d => d.Display).AsNoTracking()
                .Include(d => d.DisplayGroupConfigurations).ThenInclude(d => d.DisplayGroup).AsNoTracking()
                .SingleAsync(d => d.DisplayConfigurationId == displayConfigurationId);
        }

        /// <summary>
        /// Gets the displayConfiguration by identifier.
        /// </summary>
        /// <param name="displayGroupId">The display group identifier.</param>
        /// <returns>
        /// DisplayConfiguration
        /// </returns>
        public async Task<IEnumerable<DisplayConfiguration>> GetDisplayGroupConfigurationsByDisplayGroupId(int displayGroupId)
        {
            return await _auctionsContext.DisplayConfigurations.Include(d => d.Display).AsNoTracking()
                .Where(d => d.DisplayGroupConfigurations.Any(g => g.DisplayGroupId == displayGroupId))
                .ToListAsync();
        }

        /// <summary>
        /// Gets the display configurations by auction session identifier.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DisplayConfiguration>> GetDisplayConfigurations()
        {
            return await _auctionsContext.DisplayConfigurations
                .Include(d => d.Display)
                .Include(d => d.DisplayGroupConfigurations)
                .ThenInclude(d => d.DisplayGroup)
                .ToListAsync();
        }

        /// <summary>
        /// Updates the display configuration.
        /// </summary>
        /// <param name="displayConfigurationId">the id of the display configuration to update</param>
        /// <param name="displayConfiguration">The display configuration.</param>
        /// <returns></returns>
        public async Task<DisplayConfiguration> UpdateDisplayConfiguration(int displayConfigurationId, DisplayConfiguration displayConfiguration)
        {
            var displayConfigurationInDb = _auctionsContext.DisplayConfigurations.Include(d => d.DisplayGroupConfigurations).ThenInclude(d => d.DisplayGroup)
                .Single(d => d.DisplayConfigurationId == displayConfigurationId);
            _auctionsContext.Entry(displayConfigurationInDb).CurrentValues.SetValues(displayConfiguration);

            foreach (var typeInDb in displayConfigurationInDb.DisplayGroupConfigurations.ToList())
                if (!displayConfiguration.DisplayGroupConfigurations.Any(d => d.DisplayGroupId == typeInDb.DisplayGroupId))
                    displayConfigurationInDb.DisplayGroupConfigurations.Remove(typeInDb);

            if(displayConfiguration.DisplayGroupConfigurations != null)
            {
                foreach (var type in displayConfiguration.DisplayGroupConfigurations)
                    if (!displayConfigurationInDb.DisplayGroupConfigurations.Any(d => d.DisplayGroupId == type.DisplayGroupId))
                    {
                        _auctionsContext.DisplayGroupConfigurations.Attach(type);
                        displayConfigurationInDb.DisplayGroupConfigurations.Add(type);
                    }
            }
            
            await _auctionsContext.SaveChangesAsync();
            return displayConfiguration;
        }

        /// <summary>
        /// Deletes the display configuration.
        /// </summary>
        /// <param name="displayConfigurationId">The display configuration identifier.</param>
        /// <returns></returns>
        public async Task DeleteDisplayConfiguration(int displayConfigurationId)
        {
            var displayConfigurationToDelete = _auctionsContext.DisplayConfigurations
                .Include(d => d.DisplayGroupConfigurations)
                .Single(d => d.DisplayConfigurationId == displayConfigurationId);
            _auctionsContext.Entry(displayConfigurationToDelete).State = EntityState.Deleted;
            await _auctionsContext.SaveChangesAsync();
        }
    }
}
