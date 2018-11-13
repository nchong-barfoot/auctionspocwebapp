using AutoMapper;
using BT.Auctions.API.Data.Interfaces;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Helpers;
using BT.Auctions.API.Services.Interfaces;
using BT.Auctions.API.Services.Pipelines.Displays.ManageDisplayLogic;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services
{
    /// <summary>
    /// Display Service to handle logic and pipeline execution for Display endpoint
    /// </summary>
    /// <seealso cref="BT.Auctions.API.Services.Interfaces.IDisplayService" />
    public class DisplayService : IDisplayService
    {
        private readonly AddDisplayPipeline _addDisplayLogicPipeline;
        private readonly UpdateDisplayPipeline _updateDisplayLogicPipeline;
        private readonly DeleteDisplayPipeline _deleteDisplayLogicPipeline;

        private readonly IDisplayRepository _displayRepository;
        private readonly IAuctionSessionRepository _auctionSessionRepository;
        private readonly IBidRepository _bidRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<ServiceSettings> _serviceSettings;
        private readonly IDataProtector _protector;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="displayRepository">The display repository.</param>
        /// <param name="venueRepository">The venue repository.</param>
        /// <param name="auctionSessionRepository">The auction session repository.</param>
        /// <param name="bidRepository">The bid repository.</param>
        /// <param name="mediaRepository">The media repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="serviceSettings">The service settings.</param>
        /// <param name="provider">The provider.</param>
        public DisplayService(ILogger<Display> logger, IDisplayRepository displayRepository, IVenueRepository venueRepository, IAuctionSessionRepository auctionSessionRepository, IBidRepository bidRepository, IMediaRepository mediaRepository, IMapper mapper, IOptions<ServiceSettings> serviceSettings, IDataProtectionProvider provider)
        {
            _displayRepository = displayRepository;
            var venueRepository1 = venueRepository;
            _auctionSessionRepository = auctionSessionRepository;
            _bidRepository = bidRepository;
            _mediaRepository = mediaRepository;
            _mapper = mapper;
            _serviceSettings = serviceSettings;
            _protector = provider.CreateProtector(_serviceSettings.Value.DisplaySecret);
            _updateDisplayLogicPipeline = new UpdateDisplayPipeline(logger, _displayRepository, venueRepository1);
            _addDisplayLogicPipeline = new AddDisplayPipeline(logger, _displayRepository, venueRepository1);
            _deleteDisplayLogicPipeline = new DeleteDisplayPipeline(logger, _displayRepository);
        }

        /// <summary>
        /// Posts the display.
        /// </summary>
        /// <param name="sourceDisplay">The display.</param>
        /// <returns>the added display</returns>
        public async Task<Display> AddDisplay(DisplayDto sourceDisplay)
        {
            var mappedDisplay = _mapper.Map<DisplayDto, Display>(sourceDisplay);
            return await _addDisplayLogicPipeline.Execute(mappedDisplay);
        }

        /// <summary>
        /// Deletes the display.
        /// </summary>
        /// <param name="displayId">The display identifier.</param>
        /// <returns>Display Object with cancellation reason if it applies</returns>
        public async Task<Display> DeleteDisplay(int displayId)
        {
            return await _deleteDisplayLogicPipeline.Execute(new Display { DisplayId = displayId });
        }

        /// <summary>
        /// Gets the display by identifier.
        /// </summary>
        /// <param name="displayId">The display identifier.</param>
        /// <returns></returns>
        public async Task<Display> GetDisplayById(int displayId)
        {
            return await _displayRepository.GetDisplayById(displayId);
        }

        /// <summary>
        /// Gets the display by venue identifier.
        /// </summary>
        /// <param name="venueId">The identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Display>> GetDisplaysByVenueId(int venueId)
        {
            return await _displayRepository.GetDisplaysByVenueId(venueId);
        }

        /// <summary>
        /// Gets the available displays.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<DisplayDto>> GetAvailableDisplays(int auctionSessionId)
        {
            var auctionSession = await _auctionSessionRepository.GetAuctionSessionById(auctionSessionId);
            var displays = await _displayRepository.GetDisplays();

            var displayIds = displays.Where(d => d.DisplayConfigurations
                .Any(c => c.DisplayGroupConfigurations != null &&
                          c.DisplayGroupConfigurations.Any(gc => 
                              gc.DisplayGroup.AuctionSessions
                                .Any(a => auctionSession.StartDate != null && a.StartDate != null && DateTimeHelper.CheckDateIsWithinRange(a.StartDate.Value,
                                                                                   auctionSession.StartDate.Value)))))
                .Select(d => d.DisplayId).ToList();

            var mappedDisplays = _mapper.Map<IEnumerable<DisplayDto>>(displays);
            var displayDtos = mappedDisplays as DisplayDto[] ?? mappedDisplays.ToArray();
            displayDtos.GroupBy(d => new
            {
                d.DisplayId,
                Display = d
            }).Select(d => d.Key.Display).ToList().ForEach(d => { d.IsInUse = displayIds.Contains(d.DisplayId.Value); });

            return displayDtos;
        }

        /// <summary>
        /// Gets the display lots by auction session identifier. Should only bring back lots for auction sessions happening right now
        /// this will be surfaced through an anonymous endpoint as displays won't have a login yet
        /// </summary>
        /// <param name="displayToken">The display token.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        public async Task<AuctionSession> GetDisplayAuctionSession(string displayToken, int auctionSessionId)
        {
            var displayIdUnprotected = UnprotectDisplayAccessToken(displayToken);
            if (!int.TryParse(displayIdUnprotected, out var displayId))
                return null;

            var auctionSession = await _auctionSessionRepository.GetAuctionSessionDetailsById(auctionSessionId);
            return auctionSession?.DisplayGroup?.DisplayGroupConfigurations != null && auctionSession.DisplayGroup.DisplayGroupConfigurations.Any(
                       d => d.DisplayConfiguration.DisplayId == displayId) ? auctionSession : null;
        }

        /// <summary>
        /// Gets the bids for a lot by lot identifier. Should only bring back the latest bids for a specified lot
        /// </summary>
        /// <param name="displayToken">The display token.</param>
        /// <param name="lotId">The lot identifier</param>
        /// <returns></returns>
        public async Task<IEnumerable<Bid>> GetDisplayBids(string displayToken, int lotId)
        {
            var displayIdUnprotected = UnprotectDisplayAccessToken(displayToken);
            if (!int.TryParse(displayIdUnprotected, out var displayId))
                return null;

            var bids = await _bidRepository.GetLatestBidsByLotId(lotId);
            return bids;
        }

        public async Task<IEnumerable<Media>> GetDisplayMedia(string displayToken)
        {
            var displayIdUnprotected = UnprotectDisplayAccessToken(displayToken);
            if (!int.TryParse(displayIdUnprotected, out var displayId))
                return null;

            return await _mediaRepository.GetMedia();
        }

        /// <summary>
        /// Protects the display access token.
        /// </summary>
        /// <param name="displayId">The display identifier.</param>
        /// <returns></returns>
        public string ProtectDisplayAccessToken(int displayId)
        {
            return _protector.Protect(displayId.ToString());
        }

        /// <summary>
        /// Unprotects the display access token.
        /// </summary>
        /// <param name="protectedPayload">The protected payload.</param>
        /// <returns></returns>
        public string UnprotectDisplayAccessToken(string protectedPayload)
        {
            try
            {
                return _protector.Unprotect(protectedPayload);
            }
            catch (CryptographicException)
            {
                //payload was not generated using this system and failed to decrypt
                throw new UnauthorizedAccessException("Invalid Display Token Provided");
            }
            
        }

        /// <summary>
        /// Updates the display configuration.
        /// </summary>
        /// <param name="displayId">The display configuration identifier.</param>
        /// <param name="sourceDisplay">The source display.</param>
        /// <returns></returns>
        public async Task<Display> UpdateDisplay(int displayId, DisplayDto sourceDisplay)
        {
            try
            {
                var mappedDisplay = _mapper.Map(sourceDisplay, await _displayRepository.GetDisplayById(displayId));
                return await _updateDisplayLogicPipeline.Execute(mappedDisplay);
            }
            catch (InvalidOperationException)
            {
                return await Task.FromResult(new Display()
                {
                    CancellationReason = $"Could not find Display {displayId} in System",
                    IsCancelled = true
                });
            }
        }
    }
}