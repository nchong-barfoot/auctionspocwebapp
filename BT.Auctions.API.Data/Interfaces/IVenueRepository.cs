using BT.Auctions.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Data.Interfaces
{
    /// <summary>
    /// Venue Repository Interface
    /// </summary>
    public interface IVenueRepository
    {
        Task<Venue> AddVenue(Venue venue);
        Task<Venue> GetVenueById(int id);
        Task<IEnumerable<Venue>> GetVenues();
        Task<Venue> UpdateVenue(int id, Venue venue);
        Task DeleteVenue(Venue venue);
        IQueryable<Venue> GetPagedVenues();
    }
}
