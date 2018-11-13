using System;
using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Helpers;
using BT.Auctions.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BT.Auctions.API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace BT.Auctions.API.Controllers
{
    /// <summary>
    /// Main Auction Session API endpoint for all external communication with the auctions application
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <inheritdoc />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuctionSessionsController : ControllerBase
    {
        private readonly IAuctionSessionService _auctionSessionService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuctionSessionsController" /> class.
        /// </summary>
        /// <param name="auctionSessionService">The Auction Session service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">Automapper mapping object</param>
        public AuctionSessionsController(IAuctionSessionService auctionSessionService, ILogger<AuctionSessionsController> logger, IMapper mapper)
        {
            _mapper = mapper;
            _auctionSessionService = auctionSessionService;
        }

        /// <summary>
        /// Gets all Auction Sessions currently in the system
        /// </summary>
        /// <returns>
        /// An array of Auction Sessions
        /// </returns>
        /// <response code="200">Returns a list of Auction Sessions</response>
        /// <response code="204">If no auctionSessions are found</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AuctionSessionDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetAuctionSessions()
        {
            var auctionSessions = await _auctionSessionService.GetAuctionSessions();
            if (auctionSessions != null && auctionSessions.Any())
                return Ok(auctionSessions.Select(auctionSession => _mapper.Map<AuctionSessionDto>(auctionSession)));
            return NoContent();
        }

        /// <summary>
        /// Gets all Auction Sessions currently in the system
        /// </summary>
        /// <returns>
        /// An array of Auction Sessions
        /// </returns>
        /// <response code="200">Returns a list of Auction Sessions</response>
        /// <response code="204">If no auctionSessions are found</response>
        [HttpGet]
        [Route("GetPagedAuctionSessions")]
        [ProducesResponseType(typeof(IEnumerable<AuctionSessionDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetPagedAuctionSessions(int pageNumber, int pageSize, string timeZone, DateTimeOffset? currentDate = null)
        {
            var auctionSessions = await _auctionSessionService.GetPagedAuctionSessions(pageNumber, pageSize, timeZone, currentDate);
            if (auctionSessions != null && !auctionSessions.IsCancelled)
                return Ok(auctionSessions);
            return BadRequest(auctionSessions.CancellationReason);
        }

        /// <summary>
        /// Gets the Auction Session by identifier.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns>
        /// The requested Auction Session
        /// </returns>
        /// <response code="200">Returns a requested Auction Session</response>
        /// <response code="400">If a auctionSession by the requested ID is not found</response>
        [HttpGet]
        [Route("GetAuctionSessionById/{auctionSessionId}")]
        [ProducesResponseType(typeof(AuctionSessionDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAuctionSessionById(int auctionSessionId)
        {
            var auctionSessionTask = await _auctionSessionService.GetAuctionSessionById(auctionSessionId);
            if (!auctionSessionTask.IsCancelled)
                return Ok(_mapper.Map<AuctionSessionDto>(auctionSessionTask));
            return BadRequest(auctionSessionTask.CancellationReason);
        }

        /// <summary>
        /// Adds a new Auction Session to the system
        /// </summary>
        /// <param name="auctionSession">The Auction Session object to be added</param>
        /// <returns>
        /// the created Auction Session with the system generated identifier
        /// </returns>
        /// <response code="201">Returns the created Auction Session</response>
        /// <response code="400">If the request was not valid</response>
        [HttpPost]
        [AllowAnonymous]
        [ServiceFilter(typeof(IpFilterAttribute))]
        [ProducesResponseType(typeof(AuctionSessionDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddAuctionSession(AuctionSessionDto auctionSession)
        {
            var addTask = await _auctionSessionService.AddAuctionSession(auctionSession);
            if (!addTask.IsCancelled)
                return Created("", _mapper.Map<AuctionSessionDto>(addTask));
            return BadRequest("Process has been cancelled: " + addTask.CancellationReason);
        }

        /// <summary>
        /// Updates the Auction Session.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="auctionSession">The Auction Session object containing the updated fields</param>
        /// <returns>
        /// the updated Auction Session
        /// </returns>
        /// <response code="200">Returns the updated Auction Session</response>
        /// <response code="400">If the new Auction Session update information was not valid</response>
        [HttpPut("{auctionSessionId}")]
        [ProducesResponseType(typeof(AuctionSessionDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateAuctionSession(int auctionSessionId, AuctionSessionDto auctionSession)
        {
            if (auctionSession.ArePropertiesAllNull())
                return BadRequest("No valid auction session field updates were received");
            var updateTask = await _auctionSessionService.UpdateAuctionSession(auctionSessionId, auctionSession);
            if (!updateTask.IsCancelled)
                return Ok(_mapper.Map<AuctionSessionDto>(updateTask));
            return BadRequest(updateTask.CancellationReason);
        }
    }
}
