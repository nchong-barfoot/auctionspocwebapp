using BT.Auctions.API.Data.Contexts;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DateTimeHelper = BT.Auctions.API.Models.Helpers.DateTimeHelper;

namespace BT.Auctions.API.Data.Repositories
{
    /// <inheritdoc />
    /// <summary>
    /// Auction Session repository to handle all database communications for auction session specific operations
    /// </summary>
    /// <seealso cref="T:BT.Auctions.API.Data.Repositories.IAuctionSessionRepository" />
    /// <seealso cref="T:BT.Auctions.API.Models.AuctionSession" />
    public class AuctionSessionRepository : IAuctionSessionRepository
    {
        private readonly AuctionsContext _auctionsContext;

        public AuctionSessionRepository(AuctionsContext auctionsContext)
        {
            _auctionsContext = auctionsContext;
        }

        /// <summary>
        /// Adds the Auction Session.
        /// </summary>
        /// <param name="auctionSession">The Auction Session.</param>
        /// <returns>AuctionSession</returns>
        public async Task<AuctionSession> AddAuctionSession(AuctionSession auctionSession)
        {
            PopulateAuctionSession(auctionSession);
            var addedAuctionSession = _auctionsContext.AuctionSessions.AddAsync(auctionSession);
            await _auctionsContext.SaveChangesAsync();
            return addedAuctionSession.Result.Entity;
        }

        private void PopulateAuctionSession(AuctionSession auctionSession)
        {
            if (auctionSession.DisplayGroupId.HasValue)
            {
                auctionSession.DisplayGroup = _auctionsContext.DisplayGroups
                    .Include(d => d.DisplayGroupConfigurations)
                    .ThenInclude(d => d.DisplayConfiguration)
                    .ThenInclude(d => d.Display)
                    .FirstOrDefault(a => a.DisplayGroupId == auctionSession.DisplayGroupId);
            }
        }

        /// <summary>
        /// Gets the base Auction Session by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>AuctionSession</returns>
        public async Task<AuctionSession> GetAuctionSessionById(int id)
        {
            return await _auctionsContext.AuctionSessions.AsNoTracking().SingleAsync(v => v.AuctionSessionId == id);
        }

        /// <summary>
        /// Gets the auction session details by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>AuctionSession</returns>
        public async Task<AuctionSession> GetAuctionSessionDetailsById(int id)
        {
            var session = await _auctionsContext.AuctionSessions.AsNoTracking()
                .Include(d => d.DisplayGroup)
                .ThenInclude(d => d.DisplayGroupConfigurations)
                .ThenInclude(d => d.DisplayConfiguration)
                .ThenInclude(d => d.Display).AsNoTracking()
                .Include(a => a.Lots)
                .ThenInclude(l => l.Images).AsNoTracking()
                .Include(a => a.Lots)
                .ThenInclude(l => l.LotDetail).AsNoTracking()
                .SingleAsync(v => v.AuctionSessionId == id);

            foreach (var sessionLot in session.Lots)
            {
                sessionLot.Bids = await _auctionsContext.Bids.OrderByDescending(b => b.BidId)
                    .Where(b => b.LotId == sessionLot.LotId && !b.IsRejected).AsNoTracking().Take(3).ToListAsync();
            }

            return session;
        }

        /// <summary>
        /// Gets the auction sessions by venue identifier.
        /// </summary>
        /// <param name="venueId">The venue identifier.</param>
        /// <returns>List of Auction Sessions belonging to the Venue specified</returns>
        public async Task<IEnumerable<AuctionSession>> GetAuctionSessionsByVenueId(int venueId)
        {
            return await _auctionsContext.AuctionSessions.Where(s => s.VenueId == venueId).AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Gets the auction sessions by display group identifier.
        /// </summary>
        /// <param name="displayGroupId">The display group identifier.</param>
        /// <returns>List of Auction Sessions with the same Display Group</returns>
        public async Task<IEnumerable<AuctionSession>> GetAuctionSessionsByDisplayGroupId(int displayGroupId)
        {
            return await _auctionsContext.AuctionSessions.AsNoTracking().Where(s => s.DisplayGroupId == displayGroupId).ToListAsync();
        }

        /// <summary>
        /// Gets the Auction Sessions by date time.
        /// </summary>
        /// <param name="startDate">The start date (Nullable)</param>
        /// <param name="finishDate">The finish date (Nullable)</param>
        /// <returns>
        /// List of auctionSessions active across the specified time
        /// </returns>
        public async Task<IEnumerable<AuctionSession>> GetAuctionSessionsByDateTime(DateTime? startDate, DateTime? finishDate)
        {
            var result = await _auctionsContext.AuctionSessions.AsNoTracking()
                .Where(s => (!startDate.HasValue || (startDate.HasValue && s.StartDate.HasValue && s.StartDate >= startDate))
                            && (!finishDate.HasValue || (finishDate.HasValue && s.FinishDate.HasValue && s.FinishDate <= finishDate)))
                .ToListAsync();
            return result;
        }

        /// <summary>
        /// Gets the auction sessions occuring on date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns></returns>
        public async Task<IEnumerable<AuctionSession>> GetAuctionSessionsOccuringOnDate(DateTimeOffset? date, string timeZone)
        {
            if (date.HasValue)
            {
                date = DateTimeHelper.ConvertToLocalDateTime(date.Value, timeZone);
            }

            var sessions = await _auctionsContext.AuctionSessions.AsNoTracking()
                .Include(d => d.DisplayGroup).ThenInclude(c => c.DisplayGroupConfigurations)
                .ThenInclude(c => c.DisplayConfiguration).AsNoTracking()
                .Where(a => a.StartDate.HasValue
                            && DateTimeHelper.CheckDateIsWithinRange(DateTimeHelper.ConvertToLocalDateTime(a.StartDate.Value, timeZone), date.Value)
                            && a.Lots.Any())
                .ToListAsync();

            return sessions;
        }

        /// <summary>
        /// Gets the auction sessions occuring by date. Used to check if an auction session is
        /// occuring during a given date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public async Task<IEnumerable<AuctionSession>> GetAuctionSessionsDetailsOccuringOnDate(DateTime date)
        {
            var sessions = await _auctionsContext.AuctionSessions.AsNoTracking()
                .Include(d => d.DisplayGroup).ThenInclude(c => c.DisplayGroupConfigurations)
                .ThenInclude(c => c.DisplayConfiguration).AsNoTracking()
                .Include(a => a.Lots)
                .ThenInclude(l => l.Images).AsNoTracking()
                .Include(a => a.Lots)
                .ThenInclude(l => l.LotDetail).AsNoTracking()
                .Where(a => a.StartDate.HasValue
                            && DateTimeHelper.CheckDateIsWithinRange(a.StartDate.Value, date)
                            && a.Lots.Any())
                .ToListAsync();

            //failsafe for when a lot has a large amount of bids causing slowdowns due to data transfer
            //and js object manipulation on the frontend
            //cannot put this in the includes and a global query filter is not appropriate for this
            //https://github.com/aspnet/EntityFrameworkCore/issues/1833
            foreach (var session in sessions)
            {
                foreach (var sessionLot in session.Lots)
                {
                    sessionLot.Bids = await _auctionsContext.Bids.OrderByDescending(b => b.BidId)
                        .Where(b => b.LotId == sessionLot.LotId && !b.IsRejected).AsNoTracking().Take(3).ToListAsync();
                }
            }

            return sessions;
        }

        /// <summary>
        /// Gets the Auction Sessions.
        /// </summary>
        /// <returns>List of AuctionSessions</returns>
        public async Task<IEnumerable<AuctionSession>> GetAuctionSessions()
        {
            return await _auctionsContext.AuctionSessions.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Gets the paged auction sessions based on a specified date.
        /// </summary>
        /// <param name="currentDateTime">The current date time.</param>
        /// <returns></returns>
        public IQueryable<AuctionSession> GetPagedAuctionSessions(DateTimeOffset? currentDateTime, string timeZone)
        {
            return _auctionsContext.AuctionSessions.Include(a => a.Lots)
                .Where(a => a.StartDate.HasValue
                            && DateTimeHelper.CheckDateIsWithinRange(DateTimeHelper.ConvertToLocalDateTime(a.StartDate.Value, timeZone), currentDateTime.Value)
                            && a.Lots.Any())
                .AsNoTracking()
                .AsQueryable();
        }

        /// <summary>
        /// Updates the Auction Session.
        /// </summary>
        /// <param name="auctionSessionId">the id of the Auction Session to update</param>
        /// <param name="auctionSession">The Auction Session.</param>
        /// <returns></returns>
        public async Task<AuctionSession> UpdateAuctionSession(int auctionSessionId, AuctionSession auctionSession)
        {
            //fill in the config values of the IDs passed during the PUT
            PopulateAuctionSession(auctionSession);
            _auctionsContext.Entry(auctionSession).State = EntityState.Modified;
            //clear all auction sessions that are in session with the same user. User must only have one auction session at a time
            await _auctionsContext.AuctionSessions
                .Where(a => a.IsInSession && a.AuctionSessionId != auctionSessionId && a.AuctionSessionAdmin == auctionSession.AuctionSessionAdmin)
                .ForEachAsync(a =>
                {
                    a.IsInSession = false;
                    a.FinishDate = null;
                }); 
            await _auctionsContext.SaveChangesAsync();
            return auctionSession;
        }
    }
}
