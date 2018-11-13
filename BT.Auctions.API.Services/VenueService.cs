using AutoMapper;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Pagination;
using BT.Auctions.API.Services.Interfaces;
using BT.Auctions.API.Services.Pipelines.Venues.ManageVenueLogic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services
{
    /// <summary>
    /// Service that manages Venues in the API
    /// </summary>
    /// <seealso cref="BT.Auctions.API.Services.Interfaces.IVenueService" />
    public class VenueService : IVenueService
    {
        private readonly AddVenuePipeline _addVenueLogicPipeline;
        private readonly UpdateVenuePipeline _updateVenueLogicPipeline;
        private readonly DeleteVenuePipeline _deleteVenueLogicPipeline;

        private readonly IVenueRepository _venueRepository;
        private readonly IAuctionSessionRepository _auctionSessionRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenueService"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance</param>
        /// <param name="auctionsRepository">The auctions repository.</param>
        public VenueService(ILogger<Venue> logger, IVenueRepository venueRepository, IAuctionSessionRepository auctionSessionRepository, IMapper mapper)
        {
            _venueRepository = venueRepository;
            _auctionSessionRepository = auctionSessionRepository;
            _mapper = mapper;
            _updateVenueLogicPipeline = new UpdateVenuePipeline(logger, _venueRepository);
            _addVenueLogicPipeline = new AddVenuePipeline(logger, _venueRepository);
            _deleteVenueLogicPipeline = new DeleteVenuePipeline(logger, _venueRepository, _auctionSessionRepository);
        }

        /// <summary>
        /// Posts the <see cref="Venue"/>
        /// </summary>
        /// <param name="sourceVenue">The source venue.</param>
        /// <returns><see cref="Venue"/></returns>
        public async Task<Venue> AddVenue(VenueDto sourceVenue)
        {
            var mappedVenue = _mapper.Map<VenueDto, Venue>(sourceVenue);
            return await _addVenueLogicPipeline.Execute(mappedVenue);
        }


        /// <summary>
        /// Gets the venue by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><see cref="Venue"/></returns>
        public async Task<Venue> GetVenueById(int venueId)
        {
            try
            {
                return await _venueRepository.GetVenueById(venueId);
            }
            catch (InvalidOperationException)
            {
                return await Task.FromResult(new Venue()
                {
                    CancellationReason = $"Could not find Venue {venueId} in System",
                    IsCancelled = true
                });
            }
        }

        /// <summary>
        /// Returns a collection of all the <see cref="Venue"/>s.
        /// </summary>
        /// <returns><see cref="Venue"/></returns>
        public async Task<IEnumerable<Venue>> GetVenues()
        {
            return await _venueRepository.GetVenues();
        }

        /// <summary>
        /// Gets the paged venues.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public async Task<PagedList<VenueDto>> GetPagedVenues(int pageNumber, int pageSize)
        {
            var query = _venueRepository.GetPagedVenues();
            var totalItems = query.Count();
            var venues = query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToList();
            var mappedVenues = _mapper.Map<IEnumerable<VenueDto>>(venues);
            var pagedResult = new PagedList<VenueDto>(mappedVenues, totalItems, pageNumber, pageSize);

            return pagedResult;
        }

        /// <summary>
        /// Updates a <see cref="Venue"/>
        /// </summary>
        /// <param name="id">The venue identifier.</param>
        /// <param name="venue">The <see cref="Venue"/></param>
        /// <returns><see cref="Venue"/></returns>
        public async Task<Venue> UpdateVenue(int venueId, VenueDto sourceVenue)
        {
            try
            {
                var mappedVenue = _mapper.Map(sourceVenue, await _venueRepository.GetVenueById(venueId));
                return await _updateVenueLogicPipeline.Execute(mappedVenue);
            }
            catch (InvalidOperationException)
            {
                return await Task.FromResult(new Venue()
                {
                    CancellationReason = $"Could not find Venue {venueId} in System",
                    IsCancelled = true
                });
            }
        }

        /// <summary>
        /// Deletes a <see cref="Venue"/>
        /// </summary>
        /// <param name="id">The identifier.</param>
        public async Task<Venue> DeleteVenue(int venueId)
        {
            return await _deleteVenueLogicPipeline.Execute(new Venue { VenueId = venueId });
        }
    }
}