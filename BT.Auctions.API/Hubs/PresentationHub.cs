using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Helpers;
using BT.Auctions.API.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Hubs
{
    /// <summary>
    /// The presentation hub assists in broadcasting messages sent between the administration control panel and
    /// client displays. This allows the administration control panel to control the views/videos displayed on the
    /// client displays.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.SignalR.Hub" />
    public class PresentationHub : Hub
    {
        private static readonly ConcurrentDictionary<int, HubClient> _connectedDisplays = new ConcurrentDictionary<int, HubClient>();
        private static readonly ConcurrentDictionary<string, HubControlPanel> _connectedControlPanels = new ConcurrentDictionary<string, HubControlPanel>();
        private static readonly ConcurrentDictionary<int, List<string>> _auctionSessionDisplays = new ConcurrentDictionary<int, List<string>>();
        private static readonly ConcurrentDictionary<int, HubMedia> _mediaAttributes = new ConcurrentDictionary<int, HubMedia>();
        private const string CONTROL_PANEL_ID = "ControlPanelId";
        private const string DISPLAY_TOKEN = "token";
        private const string TIMEZONE = "timeZone";
        private readonly ILogger<PresentationHub> _logger;
        private readonly IAuctionSessionService _auctionSessionService;
        private readonly ILotService _lotService;
        private readonly IDisplayService _displayService;
        private readonly IBidService _bidService;
        private readonly IOptions<HubMethods> _hubMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationHub"/> class.
        /// </summary>
        public PresentationHub(IAuctionSessionService auctionSessionService, ILotService lotService, IDisplayService displayService, IBidService bidService, ILogger<PresentationHub> logger, IOptions<HubMethods> hubMethods)
        {
            _auctionSessionService = auctionSessionService;
            _lotService = lotService;
            _bidService = bidService;
            _displayService = displayService;
            _logger = logger;
            _hubMethods = hubMethods;
        }

        /// <summary>
        /// Called when a new connection is established with the hub.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous connect.
        /// </returns>
        public override async Task OnConnectedAsync()
        {
            //verify query string DisplayId is provided, reject connection if it isn't
            if (!string.IsNullOrEmpty(Context.GetHttpContext()?.Request?.Query[DISPLAY_TOKEN]))
            {
                try
                {
                    if(!int.TryParse(
                        _displayService.UnprotectDisplayAccessToken(Context.GetHttpContext()?.Request
                            ?.Query[DISPLAY_TOKEN]), out int displayId))
                    {
                        throw new UnauthorizedAccessException();
                    }

                    string timeZone = Context.GetHttpContext()?.Request?.Query[TIMEZONE] ?? DateTimeHelper.DEFAULT_TIMEZONE;

                    await base.OnConnectedAsync();
                    if (!_connectedDisplays.TryGetValue(displayId, out _))
                    {
                        //display with provided token/Id has not been added to connected displays, allow it to be added
                        _connectedDisplays[displayId] = new HubClient
                        {
                            ConnectionId = Context.ConnectionId,
                            TimeZone = timeZone
                        };
                        await InitDisplay(displayId, timeZone);
                    }
                    else
                    {
                        //display has already been added and is currently active. Deny request and send failed message
                        _logger.LogWarning(
                            $"{(int) ErrorCodes.ACCESS_DENIED_DISPLAY_ALREADY_CONNECTED} - User tried to connect with a display ID that is currently in use. " +
                            $"Previous display will remain connected and will take precedence over newly connected displays");
                        //display may have tried to reconnect when a current display is in the process of being disconnected. 
                        //allow these displays to continue to reconnect unless the control panel specifically states disconnect display
                        await Clients.Caller.SendAsync(_hubMethods.Value.DisconnectClient.Value, true);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    //don't allow them to reconnect if the token decryption fails
                    await base.OnConnectedAsync();
                    await Clients.Caller.SendAsync(_hubMethods.Value.DisconnectClient.Value, false);
                }
            }
            else if (!string.IsNullOrEmpty(Context.GetHttpContext()?.Request?.Query[CONTROL_PANEL_ID]))
            {
                var controlPanelId = Context.GetHttpContext()?.Request?.Query[CONTROL_PANEL_ID];
                if (_connectedControlPanels.TryGetValue(controlPanelId, out var controlPanel))
                {
                    await Clients.Client(controlPanel.ConnectionId).SendAsync(_hubMethods.Value.ConnectionFailure.Value,
                        $"Disconnected. User {controlPanelId} has logged in elsewhere");
                    await Clients.Client(controlPanel.ConnectionId).SendAsync(_hubMethods.Value.DisconnectClient.Value, false);
                    //re-allocate the connection for the control panel to the newly logged in user
                    controlPanel.ConnectionId = Context.ConnectionId;
                    //control panel user found in system, update the control panel to its current state
                    if (controlPanel.AuctionSessionState == AuctionSessionState.BiddingStarted && 
                        (controlPanel.AuctionSession == null || !controlPanel.VenueId.HasValue || !controlPanel.DisplayGroupId.HasValue))
                    {
                        //reset the session state if the hub does not know everything about the venue, session and display group
                        controlPanel.AuctionSessionState = AuctionSessionState.NotStarted;
                    }
                    await Clients.Caller.SendAsync(_hubMethods.Value.SetStoreValues.Value,
                        controlPanel.AuctionSession?.AuctionSessionId,
                        controlPanel.VenueId,
                        controlPanel.DisplayGroupId,
                        controlPanel.CurrentLotId,
                        controlPanel.AuctionSessionState);
                }
                else
                {
                    _connectedControlPanels[controlPanelId] = new HubControlPanel
                    {
                        ConnectionId = Context.ConnectionId,
                        AuctionSessionState = AuctionSessionState.NotStarted
                    };
                }

                await base.OnConnectedAsync();
            }
            else
            {
                //reject all connection requests that don't specify their type
                await Clients.Caller.SendAsync(_hubMethods.Value.ConnectionFailure.Value,
                    $"Access is Denied, view logs for more information. Error: {(int)ErrorCodes.ACCESS_DENIED_DISPLAY_TYPE_NOT_SPECIFIED}");
                _logger.LogWarning($"{(int)ErrorCodes.ACCESS_DENIED_DISPLAY_TYPE_NOT_SPECIFIED} - User tried to connect without supplying either a display or control identifier. Connection rejected");
            }
        }

        /// <summary>
        /// Called when disconnected asynchronously from hub.
        /// </summary>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var display = _connectedDisplays.FirstOrDefault(d => d.Value.ConnectionId == Context.ConnectionId);
            if (display.Value != null)
            {
                await DisconnectDisplay(display.Key, display.Value.TimeZone);
                _logger.LogInformation($"Display {display.Key} was disconnected from the hub {exception}");
            }
            await base.OnDisconnectedAsync(exception);
        }

        #region Initialise Phase

        /// <summary>
        /// Sets the auction session identifier.
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        public async Task SetAuctionSessionId(string controlPanelId, int auctionSessionId)
        {
            if (!_connectedControlPanels.TryGetValue(controlPanelId, out var controlPanel)) return;

            if (controlPanel.AuctionSession != null && controlPanel.AuctionSession.IsInSession)
            {
                await _auctionSessionService.UpdateAuctionSession(controlPanel.AuctionSession.AuctionSessionId, new AuctionSessionDto
                {
                    IsInSession = false
                });
            }

            controlPanel.AuctionSession = new AuctionSession
            {
                AuctionSessionId = auctionSessionId
            };
            controlPanel.AuctionSessionState = AuctionSessionState.AuctionSessionSelected;
            var auctionSessionToUpdate = await _auctionSessionService.GetAuctionSessionById(auctionSessionId);

            await _auctionSessionService.UpdateAuctionSession(auctionSessionId, new AuctionSessionDto
            {
                AuctionSessionAdmin = controlPanelId,
                StartDate = DateTimeOffset.UtcNow,
                FinishDate = auctionSessionToUpdate.FinishDate.HasValue ? DateTimeOffset.UtcNow.AddHours(2) : (DateTimeOffset?) null,
                IsInSession = true,
                VenueId = null,
                DisplayGroupId = null
            });

            await Clients.Clients(_connectedControlPanels.Select(c => c.Value.ConnectionId).ToList())
                .SendAsync(_hubMethods.Value.UpdateAuctionSessions.Value, auctionSessionId, controlPanelId);
        }

        /// <summary>
        /// Sets the venue identifier.
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="venueId">The venue identifier.</param>
        /// <returns></returns>
        public void SetVenueId(string controlPanelId, int venueId)
        {
            if (!_connectedControlPanels.TryGetValue(controlPanelId, out var controlPanel)) return;
            controlPanel.VenueId = venueId;
            controlPanel.AuctionSessionState = AuctionSessionState.VenueSelected;
        }

        /// <summary>
        /// Sets the display group identifier.
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="displayGroupId">The display group identifier.</param>
        /// <returns></returns>
        public async Task SetDisplayGroupId(string controlPanelId, int displayGroupId)
        {
            if (!_connectedControlPanels.TryGetValue(controlPanelId, out var controlPanel))
            {
                return;
            }

            var previousDisplayGroupId = controlPanel.DisplayGroupId;
            var otherControlPanels = _connectedControlPanels.Where(c =>
                c.Value.ConnectionId != controlPanel.ConnectionId && c.Value.DisplayGroupId.HasValue).ToList();

            controlPanel.DisplayGroupId = displayGroupId;
            controlPanel.AuctionSessionState = AuctionSessionState.DisplayGroupSelected;

            if (otherControlPanels.Any())
            {
                var displayGroupsInUse = _connectedControlPanels.Select(c => c.Value.DisplayGroupId).ToList();

                if (displayGroupsInUse.Any())
                {
                    await Clients.Clients(otherControlPanels.Select(c => c.Value.ConnectionId).ToList())
                        .SendAsync(_hubMethods.Value.UpdateDisplayGroups.Value, previousDisplayGroupId,
                            displayGroupsInUse);
                }
            }
        }

        /// <summary>
        /// Initializes the display.
        /// </summary>
        /// <param name="displayId">The display identifier.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns>
        /// void
        /// </returns>
        public async Task InitDisplay(int displayId, string timeZone)
        {
            var sessions = await GetAuctionSessionsForDisplay(displayId, timeZone);
            foreach (var session in sessions)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, session.ToString());
                if (!_auctionSessionDisplays.ContainsKey(session))
                    _auctionSessionDisplays[session] = new List<string>();
                _auctionSessionDisplays[session].Add(Context.ConnectionId);
            }
        }

        /// <summary>
        /// Disconnects the display.
        /// </summary>
        /// <param name="displayId">The display identifier.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns></returns>
        private async Task DisconnectDisplay(int displayId, string timeZone)
        {
            var sessions = await GetAuctionSessionsForDisplay(displayId, timeZone);
            foreach (var session in sessions)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, session.ToString());
                _auctionSessionDisplays[session].Remove(Context.ConnectionId);
            }
            _connectedDisplays.TryRemove(displayId, out _);
        }

        /// <summary>
        /// Disconnects the display. Uses the control panel token to validate permission to execute
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="displayId">The display identifier.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns></returns>
        public async Task DisconnectDisplay(string controlPanelId, int displayId, string timeZone)
        {
            //check valid control panel ID was passed
            if (!_connectedControlPanels.TryGetValue(controlPanelId, out _)) return;

            if (_connectedDisplays.TryGetValue(displayId, out var display))
            {
                var sessions = await GetAuctionSessionsForDisplay(displayId, timeZone);

                foreach (var session in sessions)
                {
                    await Groups.RemoveFromGroupAsync(display.ConnectionId, session.ToString());
                    _auctionSessionDisplays[session].Remove(display.ConnectionId);
                }
                _connectedDisplays.TryRemove(displayId, out _);
                await Clients.Client(display.ConnectionId)
                    .SendAsync(_hubMethods.Value.ConnectionFailure.Value, $"Disconnected. Code: {(int)ErrorCodes.CONTROL_PANEL_DISCONNECTED_DISPLAY}");
                _logger.LogInformation($"Display {displayId} has been disconnected by the control panel user {controlPanelId}");
                await Clients.Client(display.ConnectionId).SendAsync(_hubMethods.Value.DisconnectClient.Value);
            }
        }

        /// <summary>
        /// Identifies the display.
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="displayId">The display identifier.</param>
        /// <returns></returns>
        public async Task IdentifyDisplay(string controlPanelId, int displayId)
        {
            //check valid control panel ID was passed
            if (!_connectedControlPanels.TryGetValue(controlPanelId, out _)) return;

            if (_connectedDisplays.TryGetValue(displayId, out var display))
            {
                var displayData = await _displayService.GetDisplayById(displayId);
                await Clients.Client(display.ConnectionId)
                    .SendAsync(_hubMethods.Value.IdentifyDisplay.Value, $"Display ID: {displayData.DisplayId} - Display Name: {displayData.DisplayName}");
            }
        }

        private async Task<IEnumerable<int>> GetAuctionSessionsForDisplay(int displayId, string timeZone)
        {
            var auctionSessions = await _auctionSessionService.GetAuctionSessionsOccuringOnDate(DateTimeOffset.UtcNow, timeZone);
            var auctionSessionsHappeningNow = auctionSessions.Where(a => a.IsInSession);
            var auctionSessionsForDisplay = new List<int>();
            foreach (var session in auctionSessionsHappeningNow)
            {
                if (session.DisplayGroup == null)
                    continue;
                //only add the display to the auction session if it is marked as active
                if (session.DisplayGroup.DisplayGroupConfigurations.Any(s => s.DisplayConfiguration.IsActive
                && s.DisplayConfiguration.DisplayId == displayId))
                {
                    auctionSessionsForDisplay.Add(session.AuctionSessionId);
                    await Clients.Caller.SendAsync(_hubMethods.Value.InitialiseSessionAndCache.Value,
                        session.AuctionSessionId);
                }
            }
            return auctionSessionsForDisplay;
        }

        /// <summary>
        /// Initializes the auction session.
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="venueId">The venue identifier.</param>
        /// <param name="displayGroupId">The display group identifier.</param>
        /// <returns>
        /// void
        /// </returns>
        public async Task InitAuctionSession(string controlPanelId, int? auctionSessionId, int? venueId, int? displayGroupId)
        {
            if (IsValidAuctionSession(auctionSessionId) && venueId.HasValue && displayGroupId.HasValue && _connectedControlPanels.TryGetValue(controlPanelId, out var controlPanel))
            {
                var sessionResult = await _auctionSessionService.UpdateAuctionSession(auctionSessionId.Value, new AuctionSessionDto
                {
                    VenueId = venueId.Value,
                    DisplayGroupId = displayGroupId.Value,
                });

                if (controlPanel.AuctionSession == null)
                {
                    controlPanel.AuctionSession = new AuctionSession
                    {
                        AuctionSessionId = auctionSessionId.Value
                    };
                }

                if (!sessionResult.IsCancelled)
                {
                    //put displays from the auction session into the auction session group if they are connected
                    await InitAuctionSessionDisplays(auctionSessionId);

                    //reset all clients in the auction session to reset cache values
                    await RefreshClients(auctionSessionId);
                }
                else
                {
                    await Clients.Caller.SendAsync(_hubMethods.Value.ValidationMessage.Value, sessionResult.CancellationReason);
                }
            }
        }

        /// <summary>
        /// Initializes the auction session displays.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        public async Task InitAuctionSessionDisplays(int? auctionSessionId)
        {
            var session = await _auctionSessionService.GetAuctionSessionById(auctionSessionId.Value);

            //remove all previously added displays if they exist as we want a fresh group on each initialise
            if (_auctionSessionDisplays.TryGetValue(auctionSessionId.Value, out var auctionDisplays))
            {
                foreach (var connectionId in auctionDisplays)
                {
                    await Groups.RemoveFromGroupAsync(connectionId, auctionSessionId.Value.ToString());
                }
                _auctionSessionDisplays[auctionSessionId.Value] = new List<string>();
            }

            foreach (var config in session.DisplayGroup.DisplayGroupConfigurations)
            {
                if (config.DisplayConfiguration == null)
                    continue;

                if (!_auctionSessionDisplays.ContainsKey(session.AuctionSessionId))
                    _auctionSessionDisplays[session.AuctionSessionId] = new List<string>();

                if (_connectedDisplays.TryGetValue(config.DisplayConfiguration.DisplayId, out var displayClient))
                {
                    if (config.DisplayConfiguration.IsActive)
                    {
                        await Groups.AddToGroupAsync(displayClient.ConnectionId, auctionSessionId.ToString());
                        _auctionSessionDisplays[session.AuctionSessionId].Add(Context.ConnectionId);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the display group.
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        public async Task AuctionSessionUpdated(string controlPanelId, int? auctionSessionId)
        {
            if (!auctionSessionId.HasValue || !_connectedControlPanels.TryGetValue(controlPanelId, out var controlPanel))
            {
                //control panel user shouldn't be asking for display statuses without an auction session id
                return;
            }
            controlPanel.AuctionSession = await _auctionSessionService.GetAuctionSessionById(auctionSessionId.Value);
        }

        /// <summary>
        /// Gets the display statuses.
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="displayGroupId">The display group identifier.</param>
        /// <returns></returns>
        public async Task GetDisplayStatuses(string controlPanelId, int? auctionSessionId, int? displayGroupId)
        {
            if (!auctionSessionId.HasValue || !displayGroupId.HasValue || !_connectedControlPanels.TryGetValue(controlPanelId, out var controlPanel))
            {
                //control panel user shouldn't be asking for display statuses without an auction session id
                return;
            }

            if (controlPanel.AuctionSession?.DisplayGroup == null || controlPanel.AuctionSession.DisplayGroupId != displayGroupId.Value)
            {
                controlPanel.AuctionSession = await _auctionSessionService.GetAuctionSessionById(auctionSessionId.Value);
            }
            var displayStatuses = new ConcurrentDictionary<int, DisplayStatus>();
            if (controlPanel.AuctionSession.DisplayGroup == null)
            {
                //session display group update failed to bind, they shouldn't be checking for display statuses without this
                //send control panel user back to displaygroup selection.
                await Clients.Caller.SendAsync(_hubMethods.Value.SetStoreValues.Value,
                    auctionSessionId.Value,
                    controlPanel.VenueId,
                    null,
                    AuctionSessionState.VenueSelected);
                return;
            }
            foreach (var config in controlPanel.AuctionSession.DisplayGroup.DisplayGroupConfigurations)
            {
                //iterate over all configurations and make sure any connected clients are within the auction session group
                //keep adding connects to the session group as displays are being attached/initialised during this phase
                if (config.DisplayConfiguration == null)
                    continue;

                if (_connectedDisplays.TryGetValue(config.DisplayConfiguration.DisplayId, out var displayClient))
                {
                    //display is connected, continue to add it to the auction session group
                    await Groups.AddToGroupAsync(displayClient.ConnectionId, auctionSessionId.ToString());

                    //cache value will be reset when a new display is connected
                    displayStatuses.TryAdd(config.DisplayConfiguration.DisplayId, displayClient.IsCached ?
                        DisplayStatus.ConnectedAndCached : DisplayStatus.ConnectedAndNotCached);
                }
                else
                {
                    displayStatuses.TryAdd(config.DisplayConfiguration.DisplayId, DisplayStatus.Disconnected);
                }
            }
            await Clients.Caller.SendAsync(_hubMethods.Value.DisplayStatuses.Value, displayStatuses);
        }

        /// <summary>
        /// Finalises the display cache and switches the flag after a display has confirmed cache
        /// </summary>
        /// <param name="displayToken">The display token.</param>
        public void FinaliseDisplayCache(string displayToken)
        {
            var token = _displayService.UnprotectDisplayAccessToken(displayToken);
            if (!string.IsNullOrEmpty(token) && int.TryParse(token, out var displayId))
            {
                _connectedDisplays.Where(d => d.Key == displayId).ToList().ForEach(d => d.Value.IsCached = true);
            }
        }

        /// <summary>
        /// Refreshes the clients.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        public async Task RefreshClients(int? auctionSessionId)
        {
            if (IsValidAuctionSession(auctionSessionId))
            {
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.RefreshClients.Value, auctionSessionId.Value, null);
            }
        }

        #endregion

        #region Auction Session Phase
        /// <summary>
        /// Completes the auction session.
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        public async Task CompleteAuctionSession(string controlPanelId, int? auctionSessionId)
        {
            if (IsValidAuctionSession(auctionSessionId) && _connectedControlPanels.TryGetValue(controlPanelId, out _))
            {
                await _auctionSessionService.UpdateAuctionSession(auctionSessionId.Value, new AuctionSessionDto
                {
                    FinishDate = DateTimeOffset.UtcNow,
                    IsInSession = false
                });

                //reset the control panel values as they have finished the auction session
                _connectedControlPanels[controlPanelId] = new HubControlPanel
                {
                    ConnectionId = Context.ConnectionId,
                    AuctionSessionState = AuctionSessionState.NotStarted
                };

                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.CompleteAuctionSession.Value);
            }
        }

        /// <summary>
        /// Changes to media view specifically
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="mediaId">The media identifier.</param>
        /// <returns></returns>
        public async Task ChangeToMediaView(string controlPanelId, int? auctionSessionId, int? mediaId)
        {
            if (IsValidAuctionSession(auctionSessionId) && _connectedControlPanels.TryGetValue(controlPanelId, out _))
            {
                var auctionSession = await _auctionSessionService.GetAuctionSessionById(auctionSessionId.Value);
                //if Media view is requested then send time sync start message
                //we need to send different messages depending on the configuration for the display

                foreach (var configuration in auctionSession.DisplayGroup.DisplayGroupConfigurations)
                {
                    var displayConfiguration = configuration.DisplayConfiguration;
                    if (displayConfiguration == null)
                        continue;

                    //if the display is being tracked and is connected, send them the display mode they've been configured for
                    if (displayConfiguration.IsActive && displayConfiguration.PlayVideo &&
                        _connectedDisplays.TryGetValue(displayConfiguration.Display.DisplayId, out var client))
                    {
                        var connectionOptions = new Dictionary<string, string>();
                        if (configuration.DisplayConfiguration.PlayAudio)
                        {
                            connectionOptions.Add(_hubMethods.Value.EnableAudio.Value, "true");
                        }

                        if (mediaId.HasValue)
                        {
                            connectionOptions.Add(_hubMethods.Value.ChangeMedia.Value, mediaId.Value.ToString());
                        }

                        await Clients.Client(client.ConnectionId).SendAsync(_hubMethods.Value.ChangeView.Value,
                            "Media",
                            auctionSessionId.Value, connectionOptions);
                    }
                }
            }
        }

        /// <summary>
        /// Changes the view for all displays connected in the auction session
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        public async Task ChangeView(string controlPanelId, int? auctionSessionId, string viewName)
        {
            if (IsValidAuctionSession(auctionSessionId) && _connectedControlPanels.TryGetValue(controlPanelId, out var controlPanel))
            {
                var displayMode = Enum.Parse<DisplayMode>(viewName);
                var auctionSession = await _auctionSessionService.GetAuctionSessionById(auctionSessionId.Value);

                switch (displayMode)
                {
                    case DisplayMode.Bidding:
                        if (auctionSession.DisplayGroup == null)
                        {
                            //an error occurred during display group setting. Send control panel back. 
                            controlPanel.AuctionSessionState = AuctionSessionState.VenueSelected;
                            await Clients.Caller.SendAsync(_hubMethods.Value.SetStoreValues.Value,
                                auctionSessionId.Value,
                                controlPanel.VenueId,
                                null,
                                AuctionSessionState.VenueSelected);
                            return;
                        }
                        controlPanel.AuctionSessionState = AuctionSessionState.BiddingStarted;
                        foreach (var configuration in auctionSession.DisplayGroup.DisplayGroupConfigurations)
                        {
                            var displayConfiguration = configuration.DisplayConfiguration;
                            if (displayConfiguration == null)
                                continue;

                            //if the display is being tracked and is connected, send them the display mode they've been configured for
                            if (displayConfiguration.IsActive && _connectedDisplays.TryGetValue(displayConfiguration.Display.DisplayId, out var client))
                            {
                                await Clients.Client(client.ConnectionId).SendAsync(_hubMethods.Value.ChangeView.Value,
                                    configuration.DisplayConfiguration.DisplayMode.ToString(), auctionSessionId.Value);
                            }
                        }
                        break;

                    default:
                        await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.ChangeView.Value, viewName, auctionSessionId.Value);
                        break;
                }
            }
        }
        #endregion

        #region Media Phase
        /// <summary>
        /// Changes the media.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="mediaId">The media identifier.</param>
        /// <returns></returns>
        public async Task ChangeMedia(int? auctionSessionId, int? mediaId)
        {
            if (IsValidAuctionSession(auctionSessionId) && mediaId.HasValue && _mediaAttributes.TryGetValue(auctionSessionId.Value, out var media))
            {
                media.MediaId = mediaId.Value.ToString();
                _mediaAttributes[auctionSessionId.Value] = media;
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.ChangeMedia.Value, mediaId);
            }
        }

        /// <summary>
        /// Gets the current media time.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        public async Task GetCurrentMediaTime(int? auctionSessionId)
        {
            if (IsValidAuctionSession(auctionSessionId) && _mediaAttributes.TryGetValue(auctionSessionId.Value, out var media)
                && DateTime.TryParseExact(media.StartDateTime, DateTimeHelper.DEFAULT_DATE_FORMAT, CultureInfo.GetCultureInfo(DateTimeHelper.DEFAULT_CULTURE),
                DateTimeStyles.None, out var startDate))
            {
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.ResumeMedia.Value,
                    startDate.ToString(DateTimeHelper.DEFAULT_DATE_FORMAT));
            }
        }

        /// <summary>
        /// Pauses the media.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        public async Task PauseMedia(int? auctionSessionId)
        {
            if (IsValidAuctionSession(auctionSessionId) && _mediaAttributes.TryGetValue(auctionSessionId.Value, out var media)
            && DateTime.TryParseExact(media.StartDateTime, DateTimeHelper.DEFAULT_DATE_FORMAT, CultureInfo.GetCultureInfo(DateTimeHelper.DEFAULT_CULTURE),
                DateTimeStyles.None, out var startDate))
            {
                media.ElapsedVideoTime = DateTimeHelper.GetCurrentServerDateTime().Subtract(startDate);
                _mediaAttributes[auctionSessionId.Value] = media;
                await Clients.Caller.SendAsync(_hubMethods.Value.PauseMedia.Value);
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.PauseMedia.Value);
            }
        }

        /// <summary>
        /// Unpauses media.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        public async Task UnpauseMedia(int? auctionSessionId)
        {
            if (IsValidAuctionSession(auctionSessionId) && _mediaAttributes.TryGetValue(auctionSessionId.Value, out var media)
                && media.ElapsedVideoTime.HasValue)
            {
                var startDate = DateTimeHelper.GetCurrentServerDateTime().Subtract(media.ElapsedVideoTime.Value);
                media.StartDateTime = startDate.ToString(DateTimeHelper.DEFAULT_DATE_FORMAT);
                await Clients.Caller.SendAsync(_hubMethods.Value.ResumeMedia.Value, media.StartDateTime);
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.ResumeMedia.Value, media.StartDateTime);
            }
        }

        /// <summary>
        /// Starts the media.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="media">The media object with defined media index and start/end times.</param>
        /// <returns></returns>
        public async Task StartMedia(int auctionSessionId, HubMedia media)
        {
            if (IsValidAuctionSession(auctionSessionId))
            {
                _mediaAttributes[auctionSessionId] = media;
                await Clients.Caller.SendAsync(_hubMethods.Value.ResumeMedia.Value, media.StartDateTime);
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.ResumeMedia.Value, media.StartDateTime);
            }
        }

        /// <summary>
        /// Synchronizes the control panel clock.
        /// </summary>
        /// <param name="controlPanelId">The control panel identifier.</param>
        /// <param name="timeStamp">The time stamp.</param>
        public async Task SyncControlPanelClock(string controlPanelId, HubClientTimeStamp timeStamp)
        {
            if (_connectedControlPanels.TryGetValue(controlPanelId, out var controlPanel))
            {
                controlPanel.HubClient = await SyncClock(controlPanel.HubClient, timeStamp);
            }

        }

        /// <summary>
        /// Synchronizes the display clock.
        /// </summary>
        /// <param name="displayToken">The display token.</param>
        /// <param name="timeStamp">The time stamp.</param>
        /// <returns></returns>
        public async Task SyncDisplayClock(string displayToken, HubClientTimeStamp timeStamp)
        {
            if (!int.TryParse(_displayService.UnprotectDisplayAccessToken(displayToken), out int displayId))
            {
                return;
            }
            if (_connectedDisplays.TryGetValue(displayId, out var client))
            {
                _connectedDisplays[displayId] = await SyncClock(client, timeStamp);
            }

        }

        /// <summary>
        /// Synchronizes the clock for media sync between multiple clients
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="timeStamp">The time stamp.</param>
        /// <returns></returns>
        private Task<HubClient> SyncClock(HubClient client, HubClientTimeStamp timeStamp)
        {
            if (client == null)
            {
                client = new HubClient();
            }
            //reset the locally stored client time stamp if the server sends through a new sync request (with new default MaxDifference value)
            if (!timeStamp.MaxDifference.HasValue)
            {
                client.TimeStamp = new HubClientTimeStamp();
            }

            var dateConversion = DateTimeHelper.IsValidServerDate(timeStamp.Timing);
            //if date fails to convert then Item1 will be false
            if (!dateConversion.Item1)
            {
                Clients.Caller.SendAsync(_hubMethods.Value.SyncMessage.Value, $"Failed to convert date {timeStamp.Timing}");
                _logger.LogWarning($"Failed to convert client date {timeStamp.Timing} for Time Sync operation {client.ConnectionId}");
            }

            if (!string.IsNullOrEmpty(timeStamp.Timing))
            {
                var serverReceived = DateTimeHelper.GetCurrentServerDateTime();
                client.TimeStamp.MaxDifference = Math.Max(client.TimeStamp.MaxDifference ?? double.NegativeInfinity,
                    dateConversion.Item2.Subtract(serverReceived).TotalMilliseconds);

                //create new object here and send it to let Clients know how accurate they are compared to server
                timeStamp.MaxDifference = client.TimeStamp.MaxDifference;

                Clients.Caller.SendAsync(_hubMethods.Value.SyncMessage.Value, JsonConvert.SerializeObject(new
                {
                    MaxDifference = client.TimeStamp.MaxDifference.Value,
                    Timing = DateTimeHelper.GetCurrentServerDateTimeFormatted()
                }));

                if (timeStamp.MinDifference != 0)
                {
                    client.TimeStamp.MinDifference = timeStamp.MinDifference;
                    //difference being the average time of a single trip from client to server i.e. differences / 2
                    client.TimeStamp.TimingDifference = client.TimeStamp.MinDifference
                                                        + (client.TimeStamp.MaxDifference - client.TimeStamp.MinDifference) / 2;
                }
                else
                {
                    client.TimeStamp.TimingDifference = timeStamp.MaxDifference;
                }
            }
            return Task.FromResult(client);
        }

        #endregion

        #region Bidding Phase

        /// <summary>
        /// Submits the bid.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="lotId">The lot identifier.</param>
        /// <param name="bidAmount">The bid amount posted</param>
        /// <returns></returns>
        public async Task SetLotBid(int? auctionSessionId, int? lotId, string bidAmount)
        {
            if (IsValidAuctionSession(auctionSessionId) && lotId.HasValue && Decimal.TryParse(bidAmount, out var bidResult))
            {
                var lot = await _lotService.GetLotById(lotId.Value);
                if (lot == null)
                    return;

                var bid = new BidDto
                {
                    Amount = bidResult,
                    LotId = lot.LotId,
                    ReserveMet = lot.ReserveMet
                };

                await _bidService.AddBid(bid);
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.SetLotBid.Value, lotId, bidAmount);
            }
        }

        /// <summary>
        /// Reverts the lot bid.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        public async Task RevertLotBid(int? auctionSessionId, int? lotId)
        {
            if (IsValidAuctionSession(auctionSessionId) && lotId.HasValue)
            {
                var bids = (await _bidService.GetLatestBidsByLotId(lotId.Value)).ToList();
                var currentBid = bids.FirstOrDefault();
                if (currentBid != null)
                {
                    await _bidService.UpdateBid(currentBid.BidId, new BidDto
                    {
                        IsRejected = true
                    });
                }

                bids.Remove(currentBid);
                await Clients.Caller.SendAsync(_hubMethods.Value.RevertLotBid.Value, lotId, bids);
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.RevertLotBid.Value, lotId, bids);
            }
        }

        /// <summary>
        /// Sets the status of a lot.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="lotId">The lot identifier.</param>
        /// <param name="status">The status to change to</param>
        /// <param name="hideSalePrice">if set to <c>true</c> [hide sale price].</param>
        /// <returns></returns>
        public async Task SetLotStatus(int? auctionSessionId, int? lotId, int status, bool hideSalePrice)
        {
            var soldStatuses = new List<AuctionStatus>{ AuctionStatus.Sold, AuctionStatus.SoldPrior };
            var reserveMetStatuses = new List<AuctionStatus>{ AuctionStatus.Selling, AuctionStatus.Sold, AuctionStatus.SoldPrior };

            if (IsValidAuctionSession(auctionSessionId) && lotId.HasValue && Enum.TryParse<AuctionStatus>(status.ToString(), out var auctionStatus))
            {
                var sessionLots = await _lotService.GetLotsByAuctionSessionId(auctionSessionId.Value);
                var lot = sessionLots.FirstOrDefault(l => l.LotId == lotId.Value);

                var reserveMet = false;

                if (lot == null)
                    return;
                var currentBid = lot.Bids.Where(b => !b.IsRejected).OrderByDescending(b => b.BidId).FirstOrDefault();

                var lotDto = new LotDto
                {
                    AuctionStatus = auctionStatus,
                    IsSalePriceHidden = hideSalePrice
                };

                // Assigns a date to the SoldDate property if the auction
                // status is set to sold

                if (soldStatuses.Any(s => s == auctionStatus))
                {
                    lotDto.SoldDate = DateTime.UtcNow;
                    if (currentBid != null)
                        lotDto.SoldPrice = currentBid.Amount;
                }
                else
                {
                    // Nullifies both the sold date and sold price properties
                    // if the status is neither Sold nor SoldPrior
                    lotDto.SoldDate = null;
                    lotDto.SoldPrice = null;
                }

                // Sets the reserveMet flag on the lot record if the auction status is
                // Selling, Sold or SoldPrior 
                if (reserveMetStatuses.Any(s => s == auctionStatus))
                {
                    reserveMet = true;
                }

                lotDto.ReserveMet = reserveMet;

                if (currentBid != null)
                {
                    await _bidService.UpdateBid(currentBid.BidId, new BidDto
                    {
                        ReserveMet = reserveMet
                    });
                }

                await _lotService.UpdateLot(lot.LotId, lotDto);
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.SetLotStatus.Value, lotId, status, hideSalePrice);
                await Clients.Caller.SendAsync(_hubMethods.Value.SetLotStatus.Value, lotId, status, hideSalePrice);
            }
        }

        /// <summary>
        /// Changes the lot.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="lotId">The lot to change to</param>
        /// <returns></returns>
        public async Task ChangeLot(string controlPanelId, int? auctionSessionId, int? lotId)
        {
            if (IsValidAuctionSession(auctionSessionId) && lotId.HasValue && _connectedControlPanels.TryGetValue(controlPanelId, out var controlPanel))
            {
                var lot = await _lotService.GetLotById(lotId.Value);
                if (lot == null)
                    return;
                controlPanel.CurrentLotId = lotId;
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.ChangeLot.Value, lotId);
            }
        }

        /// <summary>
        /// Pauses the lot.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="lotId">The lot identifier.</param>
        /// <param name="isPaused">if set to <c>true</c> [is paused].</param>
        /// <returns></returns>
        public async Task SetPauseStatus(int? auctionSessionId, int? lotId, bool isPaused)
        {
            if (IsValidAuctionSession(auctionSessionId) && lotId.HasValue)
            {
                var lot = await _lotService.GetLotById(lotId.Value);

                if (lot == null)
                    return;

                var lotDto = new LotDto
                {
                    IsPaused = isPaused
                };

                await _lotService.UpdateLot(lot.LotId, lotDto);
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.SetPauseStatus.Value, lotId, isPaused);
            }
        }

        /// <summary>
        /// Sets the image.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        public async Task SetImage(int? auctionSessionId, int? imageId)
        {
            if (IsValidAuctionSession(auctionSessionId) && imageId.HasValue)
            {
                await Clients.Group(auctionSessionId.ToString()).SendAsync(_hubMethods.Value.SetImage.Value, imageId);
            }
        }
        #endregion


        private bool IsValidAuctionSession(int? auctionSessionId)
        {
            if (!auctionSessionId.HasValue)
            {
                _logger.LogWarning($"Auction Session ID was not specified in the Message Request. " +
                    $"{System.Reflection.MethodBase.GetCurrentMethod().Name}");
                return false;
            }
            return true;
        }

    }
}