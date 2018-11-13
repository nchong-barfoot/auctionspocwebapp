using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BT.Auctions.API.Models;

namespace BT.Auctions.API.Data.Interfaces
{
    /// <summary>
    /// AuctionSession Repository Interface
    /// </summary>
    public interface IAuctionSessionRepository
    {
        Task<AuctionSession> AddAuctionSession(AuctionSession venue);
        Task<AuctionSession> GetAuctionSessionById(int id);
        Task<AuctionSession> GetAuctionSessionDetailsById(int id);
        Task<IEnumerable<AuctionSession>> GetAuctionSessionsByDateTime(DateTime? startDate, DateTime? finishDate);
        Task<IEnumerable<AuctionSession>> GetAuctionSessionsOccuringOnDate(DateTimeOffset? date, string timeZone);
        Task<IEnumerable<AuctionSession>> GetAuctionSessions();
        Task<AuctionSession> UpdateAuctionSession(int id, AuctionSession venue);
        Task<IEnumerable<AuctionSession>> GetAuctionSessionsByVenueId(int venueId);
        Task<IEnumerable<AuctionSession>> GetAuctionSessionsByDisplayGroupId(int displayGroupId);
        IQueryable<AuctionSession> GetPagedAuctionSessions(DateTimeOffset? currentDateTime, string timeZone);
    }
}
