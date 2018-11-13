using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Services.Interfaces
{
    public interface IDisplayService
    {
        /// <summary>
        /// Adds the lot.
        /// </summary>
        /// <param name="display">The Display.</param>
        /// <returns>The Added Display</returns>
        Task<Display> AddDisplay(DisplayDto displayDto);

        /// <summary>
        /// Gets the display by identifier.
        /// </summary>
        /// <param name="displayId">The display identifier.</param>
        /// <returns></returns>
        Task<Display> GetDisplayById(int displayId);

        /// <summary>
        /// Gets the display configuration by Venue.
        /// </summary>
        /// <param name="venueId">The identifier.</param>
        /// <returns>Collection of Displays</returns>
        Task<IEnumerable<Display>> GetDisplaysByVenueId(int venueId);
        
        /// <summary>
        /// Protects the display access token.
        /// </summary>
        /// <param name="displayId">The display identifier.</param>
        /// <returns></returns>
        string ProtectDisplayAccessToken(int displayId);

        /// <summary>
        /// Unprotects the display access token.
        /// </summary>
        /// <param name="protectedPayload">The protected payload.</param>
        /// <returns></returns>
        string UnprotectDisplayAccessToken(string protectedPayload);

        /// <summary>
        /// Updates the Display.
        /// </summary>
        /// <param name="displayId">The display configuration identifier.</param>
        /// <param name="displayDto">The display configuration dto.</param>
        /// <returns>
        /// The Updated Display
        /// </returns>
        Task<Display> UpdateDisplay(int displayId, DisplayDto displayDto);

        /// <summary>
        /// Deletes the display.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<Display> DeleteDisplay(int id);

        /// <summary>
        /// Gets the available displays.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        Task<IEnumerable<DisplayDto>> GetAvailableDisplays(int auctionSessionId);

        /// <summary>
        /// Gets the display lots by auction session identifier.
        /// </summary>
        /// <param name="displayToken">The display token.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        Task<AuctionSession> GetDisplayAuctionSession(string displayToken, int auctionSessionId);

        /// <summary>
        /// Gets the bids for a lot by lot identifier. Should only bring back the latest bids for a specified lot
        /// </summary>
        /// <param name="displayToken">The display token.</param>
        /// <param name="lotId">The lot identifier</param>
        /// <returns></returns>
        Task<IEnumerable<Bid>> GetDisplayBids(string displayToken, int lotId);

        /// <summary>
        /// Gets the display media.
        /// </summary>
        /// <param name="displayToken">The display token.</param>
        /// <returns></returns>
        Task<IEnumerable<Media>> GetDisplayMedia(string displayToken);
    }
}