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
    /// Display repository to handle all database communications for display specific operations
    /// </summary>
    /// <seealso cref="T:BT.Auctions.API.Data.Repositories.IDisplayRepository" />
    /// <seealso cref="T:BT.Auctions.API.Models.Display" />
    public class DisplayRepository : IDisplayRepository
    {
        private readonly AuctionsContext _auctionsContext;

        public DisplayRepository(AuctionsContext auctionsContext)
        {
            _auctionsContext = auctionsContext;
        }

        /// <summary>
        /// Adds the display.
        /// </summary>
        /// <param name="display">The display to be added.</param>
        /// <returns>Display</returns>
        public async Task<Display> AddDisplay(Display display)
        {
            var addedDisplay = _auctionsContext.Displays.AddAsync(display);
            await _auctionsContext.SaveChangesAsync();
            return addedDisplay.Result.Entity;
        }

        /// <summary>
        /// Gets the display by identifier.
        /// </summary>
        /// <param name="displayId">The identifier.</param>
        /// <returns>Display</returns>
        public async Task<Display> GetDisplayById(int displayId)
        {
            return await _auctionsContext.Displays.SingleAsync(v => v.DisplayId == displayId);
        }

        /// <summary>
        /// Gets all displays.
        /// </summary>
        /// <returns>List of Displays</returns>
        public async Task<IEnumerable<Display>> GetDisplays()
        {
            return await _auctionsContext.Displays.AsNoTracking()
                .Include(d => d.Venue)
                .Include(d => d.DisplayConfigurations)
                .ThenInclude(c => c.DisplayGroupConfigurations)
                .ThenInclude(gc => gc.DisplayGroup)
                .ThenInclude(g => g.AuctionSessions).AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Gets the display by venue identifier.
        /// </summary>
        /// <param name="venueId">The venue identifier.</param>
        /// <returns>The requested displays</returns>
        public async Task<IEnumerable<Display>> GetDisplaysByVenueId(int venueId)
        {
            return await _auctionsContext.Displays.Include(d => d.DisplayConfigurations).ThenInclude(d => d.DisplayGroupConfigurations).Where(l => l.VenueId == venueId).ToListAsync();
        }

        /// <summary>
        /// Updates the display.
        /// </summary>
        /// <param name="displayId">the id of the display to update</param>
        /// <param name="display">The display.</param>
        /// <returns>The Updated Display</returns>
        public async Task<Display> UpdateDisplay(int displayId, Display display)
        {
            _auctionsContext.Entry(display).State = EntityState.Modified;
            await _auctionsContext.SaveChangesAsync();
            return display;
        }


        /// <summary>
        /// Deletes the display.
        /// </summary>
        /// <param name="displayId">The display identifier.</param>
        /// <returns>void task</returns>
        public async Task DeleteDisplay(int displayId)
        {
            var displayToDelete = _auctionsContext.Displays.Single(d => d.DisplayId == displayId);
            var displayConfigurationsToDelete = _auctionsContext.DisplayConfigurations
                .Include(d => d.DisplayGroupConfigurations).ThenInclude(d => d.DisplayConfiguration)
                .Include(d => d.DisplayGroupConfigurations).ThenInclude(d => d.DisplayGroup)
                .Where(d => d.DisplayId == displayId);
            foreach(var config in displayConfigurationsToDelete)
            {
                _auctionsContext.Entry(config).State = EntityState.Deleted;
            }
            _auctionsContext.Entry(displayToDelete).State = EntityState.Deleted;
            await _auctionsContext.SaveChangesAsync();
        }
    }
}
