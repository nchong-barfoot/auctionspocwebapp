using AutoMapper;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Helpers;
using BT.Auctions.API.Models.Pagination;
using BT.Auctions.API.Services.Interfaces;
using BT.Auctions.API.Services.Pipelines.AuctionSessions.ManageAuctionSessionLogic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="BT.Auctions.API.Services.Interfaces.IAuctionSessionService" />
    public class AuctionSessionService : IAuctionSessionService
    {
        private readonly AddAuctionSessionPipeline _addAuctionSessionLogicPipeline;
        private readonly UpdateAuctionSessionPipeline _updateAuctionSessionLogicPipeline;

        private readonly IAuctionSessionRepository _auctionSessionRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly IDisplayGroupRepository _displayGroupRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuctionSessionService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="auctionSessionRepository">The Auction Session repository.</param>
        /// <param name="venueRepository">The venue repository.</param>
        public AuctionSessionService(ILogger<AuctionSession> logger, IAuctionSessionRepository auctionSessionRepository, IVenueRepository venueRepository, IDisplayGroupRepository displayGroupRepository, IMapper mapper)
        {
            _auctionSessionRepository = auctionSessionRepository;
            _mapper = mapper;
            _logger = logger;
            _venueRepository = venueRepository;
            _displayGroupRepository = displayGroupRepository;
            _updateAuctionSessionLogicPipeline = new UpdateAuctionSessionPipeline(logger, _auctionSessionRepository, _venueRepository, displayGroupRepository);
            _addAuctionSessionLogicPipeline = new AddAuctionSessionPipeline(logger, _auctionSessionRepository, _venueRepository, displayGroupRepository);
        }

        /// <summary>
        /// Posts the Auction Session.
        /// </summary>
        /// <param name="auctionSession">The Auction Session.</param>
        /// <returns></returns>
        public async Task<AuctionSession> AddAuctionSession(AuctionSessionDto sourceAuctionSession)
        {
            var mappedAuctionSession = _mapper.Map<AuctionSessionDto, AuctionSession>(sourceAuctionSession);
            return await _addAuctionSessionLogicPipeline.Execute(mappedAuctionSession);
        }


        /// <summary>
        /// Gets the Auction Session by identifier.
        /// </summary>
        /// <param name="auctionSessionId">The identifier.</param>
        /// <returns></returns>
        public async Task<AuctionSession> GetAuctionSessionById(int auctionSessionId)
        {
            try
            {
                return await _auctionSessionRepository.GetAuctionSessionDetailsById(auctionSessionId);
            }
            catch (InvalidOperationException)
            {
                return await Task.FromResult(new AuctionSession()
                {
                    CancellationReason = $"Could not find Auction Session {auctionSessionId} in System",
                    IsCancelled = true
                });
            }
        }

        /// <summary>
        /// Gets the Auction Sessions.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AuctionSession>> GetAuctionSessions()
        {
            return await _auctionSessionRepository.GetAuctionSessions();
        }

        /// <summary>
        /// Gets the paged auction sessions.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <param name="currentDateTime">The current date time.</param>
        /// <returns></returns>
        public async Task<PagedList<AuctionSessionDto>> GetPagedAuctionSessions(int pageNumber, int pageSize, string timeZone, DateTimeOffset? currentDateTime)
        {
            try
            {
                if (!currentDateTime.HasValue)
                {
                    currentDateTime = DateTimeHelper.ConvertToLocalDateTime(DateTimeOffset.UtcNow, timeZone);
                }

                var query = _auctionSessionRepository.GetPagedAuctionSessions(currentDateTime.Value, timeZone);
                var auctionSessions = await Task.FromResult(query
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToList());
                var totalItems = auctionSessions.Count();
                var mappedAuctionSessions = _mapper.Map<IEnumerable<AuctionSessionDto>>(auctionSessions);
                var pagedResult =
                    new PagedList<AuctionSessionDto>(mappedAuctionSessions, totalItems, pageNumber, pageSize);

                return pagedResult;
            }
            catch (TimeZoneNotFoundException)
            {
                return await Task.FromResult(new PagedList<AuctionSessionDto>
                {
                    CancellationReason = "Invalid timezone provided",
                    IsCancelled = true
                });
            }
        }

        /// <summary>
        /// Gets the Auction Sessions over a specified DateTime range (UTC)
        /// </summary>
        /// <param name="startDate">Optional Start Date</param>
        /// <param name="finishDate">Optional Finish Date</param>
        /// <returns>Array of AuctionSessions</returns>
        public async Task<IEnumerable<AuctionSession>> GetAuctionSessionsByDate(DateTime? startDate, DateTime? finishDate)
        {
            return await _auctionSessionRepository.GetAuctionSessionsByDateTime(startDate, finishDate);
        }

        /// <summary>
        /// Gets the auction sessions occuring on the specified date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns>
        /// array of auction sessions
        /// </returns>
        public async Task<IEnumerable<AuctionSession>> GetAuctionSessionsOccuringOnDate(DateTimeOffset? date, string timeZone)
        {
            return await _auctionSessionRepository.GetAuctionSessionsOccuringOnDate(date, timeZone);
        }

        /// <summary>
        /// Updates the Auction Session.
        /// </summary>
        /// <param name="auctionSessionId">The Auction Session identifier.</param>
        /// <param name="sourceAuctionSession">The source auction session.</param>
        /// <returns></returns>
        public async Task<AuctionSession> UpdateAuctionSession(int auctionSessionId, AuctionSessionDto sourceAuctionSession)
        {
            try
            {
                var mappedAuctionSession = _mapper.Map(sourceAuctionSession,
                    await _auctionSessionRepository.GetAuctionSessionDetailsById(auctionSessionId));
                return await _updateAuctionSessionLogicPipeline.Execute(mappedAuctionSession);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.ToString());
                return await Task.FromResult(new AuctionSession
                {
                    CancellationReason = $"Could not find Auction Session {auctionSessionId} in System",
                    IsCancelled = true
                });
            }
        }
    }
}