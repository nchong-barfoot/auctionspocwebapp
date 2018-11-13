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
    /// Venue repository to handle all database communications for venue specific operations
    /// </summary>
    /// <seealso cref="T:BT.Auctions.API.Data.Repositories.IVenueRepository" />
    /// <seealso cref="T:BT.Auctions.API.Models.Venue" />
    public class VenueRepository : IVenueRepository
    {
        private readonly AuctionsContext _auctionsContext;

        public VenueRepository(AuctionsContext auctionsContext)
        {
            _auctionsContext = auctionsContext;
        }

        /// <summary>
        /// Adds the venue.
        /// </summary>
        /// <param name="venue">The venue.</param>
        /// <returns>Venue</returns>
        public async Task<Venue> AddVenue(Venue venue)
        {
            var addedVenue = _auctionsContext.Venues.AddAsync(venue);
            await _auctionsContext.SaveChangesAsync();
            return addedVenue.Result.Entity;
        }

        /// <summary>
        /// Gets the venue by identifier.
        /// </summary>
        /// <param name="venueId">The identifier.</param>
        /// <returns>Venue</returns>
        public async Task<Venue> GetVenueById(int venueId)
        {
            return await _auctionsContext.Venues.SingleAsync(v => v.VenueId == venueId);
        }

        /// <summary>
        /// Gets the venues.
        /// </summary>
        /// <returns>List of Venues</returns>
        public async Task<IEnumerable<Venue>> GetVenues()
        {
            return await _auctionsContext.Venues.ToListAsync();
        }

        /// <summary>
        /// Gets the paged venues.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Venue> GetPagedVenues()
        {
            return _auctionsContext.Venues.AsNoTracking().AsQueryable();
        }

        /// <summary>
        /// Updates the venue.
        /// </summary>
        /// <param name="venueId">the id of the venue to update</param>
        /// <param name="venue">The venue.</param>
        /// <returns></returns>
        public async Task<Venue> UpdateVenue(int venueId, Venue venue)
        {
            _auctionsContext.Entry(venue).State = EntityState.Modified;
            await _auctionsContext.SaveChangesAsync();
            return venue;
        }

        /// <summary>
        /// Deletes the venue.
        /// </summary>
        /// <param name="venue">The venue.</param>
        public async Task DeleteVenue(Venue venue)
        {
            var venueToDelete = _auctionsContext.Venues.Single(v => v.VenueId == venue.VenueId);
            _auctionsContext.Entry(venueToDelete).State = EntityState.Deleted;
            await _auctionsContext.SaveChangesAsync();
        }
    }
}
