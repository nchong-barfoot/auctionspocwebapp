using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Controllers
{
    /// <summary>
    /// Main Auction Session Result API endpoint for all external communication with the auctions application
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <inheritdoc />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuctionResultsController : ControllerBase
    {
        private readonly IAuctionSessionService _auctionSessionService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuctionSessionsController" /> class.
        /// </summary>
        /// <param name="auctionSessionService">The Auction Session service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">Automapper mapping object</param>
        public AuctionResultsController(IAuctionSessionService auctionSessionService, ILogger<AuctionSessionsController> logger, IMapper mapper)
        {
            _mapper = mapper;
            _auctionSessionService = auctionSessionService;
        }

        /// <summary>
        /// Gets all auctionSessions currently in the system
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="finishDate">The finish date.</param>
        /// <returns>
        /// An array of auctionSessions
        /// </returns>
        /// <response code="200">Returns a list of auctionSessions</response>
        /// <response code="204">If no auctionSessions are found</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AuctionSessionDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAuctionResultsByDateTime(DateTime? startDate, DateTime? finishDate)
        {
            if (startDate.HasValue && finishDate.HasValue && startDate > finishDate)
                return BadRequest("Start Date must be before Finish Date");

            var auctionSessions = await _auctionSessionService.GetAuctionSessionsByDate(startDate, finishDate);
            if (auctionSessions != null && auctionSessions.Any())
                return Ok(auctionSessions.Select(auctionSession => _mapper.Map<AuctionSessionDto>(auctionSession)));
            return NoContent();
        }
    }
}
