using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Pagination;

namespace BT.Auctions.API.Services.Interfaces
{
    public interface IVenueService
    {
        /// <summary>
        ///   Adds the <see cref="Venue" />.
        /// </summary>
        /// <param name="venue">The <see cref="Venue" /></param>
        /// <returns>
        ///   <see cref="Venue" />
        /// </returns>
        Task<Venue> AddVenue(VenueDto venue);

        /// <summary>
        ///   Gets the <see cref="Venue" /> by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<Venue> GetVenueById(int id);

        /// <summary>
        ///   Gets the <see cref="Venue" />
        /// </summary>
        /// <returns>Collection of <see cref="Venue" /></returns>
        Task<IEnumerable<Venue>> GetVenues();


        /// <summary>
        /// Gets the paged venues.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        Task<PagedList<VenueDto>> GetPagedVenues(int pageNumber, int pageSize);

        /// <summary>
        ///   Updates the <see cref="Venue"/>.
        /// </summary>
        /// <param name="id">The venue identifier.</param>
        /// <param name="venue">The <see cref="Venue"/></param>
        /// <returns><see cref="Venue"/></returns>
        Task<Venue> UpdateVenue(int id, VenueDto venue);

        /// <summary>
        ///   Deletes the <see cref="Venue"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        Task<Venue> DeleteVenue(int id);
    }
}