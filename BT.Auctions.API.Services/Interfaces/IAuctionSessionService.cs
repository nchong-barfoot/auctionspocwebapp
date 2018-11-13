using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Pagination;

namespace BT.Auctions.API.Services.Interfaces
{
    public interface IAuctionSessionService
    {
        /// <summary>
        /// Adds the Auction Session.
        /// </summary>
        /// <param name="auctionSession">The Auction Session.</param>
        /// <returns></returns>
        Task<AuctionSession> AddAuctionSession(AuctionSessionDto auctionSession);

        /// <summary>
        /// Gets the Auction Session by identifier.
        /// </summary>
        /// <param name="auctionSessionId">The identifier.</param>
        /// <returns></returns>
        Task<AuctionSession> GetAuctionSessionById(int auctionSessionId);

        /// <summary>
        /// Gets the Auction Sessions.
        /// </summary>
        /// <returns>list of auction sessions</returns>
        Task<IEnumerable<AuctionSession>> GetAuctionSessions();

        /// <summary>
        /// Gets the Auction Sessions over a specified DateTime range (UTC)
        /// </summary>
        /// <returns>list of auction sessions</returns>
        Task<IEnumerable<AuctionSession>> GetAuctionSessionsByDate(DateTime? startDate, DateTime? finishDate);

        /// <summary>
        /// Gets the paged auction sessions.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="currentDateTime">The current date time.</param>
        /// <returns></returns>
        Task<PagedList<AuctionSessionDto>> GetPagedAuctionSessions(int pageNumber, int pageSize, string timeZone, DateTimeOffset? currentDateTime);

        /// <summary>
        /// Updates the Auction Session.
        /// </summary>
        /// <param name="auctionSessionId">The Auction Session identifier.</param>
        /// <param name="auctionSession">The Auction Session.</param>
        /// <returns>gets the updated auction session</returns>
        Task<AuctionSession> UpdateAuctionSession(int auctionSessionId, AuctionSessionDto auctionSession);

        /// <summary>
        /// Gets the auction sessions occuring on the specified date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns>
        /// array of auction sessions
        /// </returns>
        Task<IEnumerable<AuctionSession>> GetAuctionSessionsOccuringOnDate(DateTimeOffset? date, string timeZone);
    }
}