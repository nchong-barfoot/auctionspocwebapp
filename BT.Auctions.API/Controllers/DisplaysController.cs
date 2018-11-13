using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Helpers;
using BT.Auctions.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BT.Auctions.API.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Main Display API endpoint for all external communication with the auctions application
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DisplaysController : ControllerBase
    {
        private readonly IDisplayService _displayService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplaysController"/> class.
        /// </summary>
        /// <param name="displayService">The Display service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">Automapper mapping object</param>
        public DisplaysController(IDisplayService displayService, ILogger<DisplaysController> logger, IMapper mapper)
        {
            _mapper = mapper;
            _displayService = displayService;
        }

        /// <summary>
        /// Gets the Display by identifier.
        /// </summary>
        /// <param name="venueId">The ID of the Venue ID that will be searched</param>
        /// <returns>The requested Display</returns>
        /// <response code="200">Returns a requested Displays</response>
        /// <response code="204">If no Displays are found for the venue</response>
        [HttpGet("{venueId}")]
        [ProducesResponseType(typeof(IEnumerable<DisplayDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetDisplayByVenueId(int venueId)
        {
            var displays = await _displayService.GetDisplaysByVenueId(venueId);
            if (displays != null && displays.Any())
                return Ok(displays.Select(display => _mapper.Map<DisplayDto>(display)));
            return NoContent();
        }

        /// <summary>
        /// Gets the available displays.
        /// </summary>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAvailableDisplays/{auctionSessionId}")]
        [ProducesResponseType(typeof(IEnumerable<DisplayDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetAvailableDisplays(int auctionSessionId)
        {
            var displays = await _displayService.GetAvailableDisplays(auctionSessionId);
            if (displays != null && displays.Any())
                return Ok(displays);
            return NoContent();
        }

        /// <summary>
        /// Gets the display lots by auction session identifier. Used by the non logged in displays
        /// and should remain open until displays are identity driven. Auction sessions
        /// will be limited to current auction sessions and require display tokens
        /// </summary>
        /// <param name="displayToken">The display token.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDisplayAuctionSession/{displayToken}/{auctionSessionId}")]
        [ProducesResponseType(typeof(IEnumerable<LotDto>), 200)]
        [ProducesResponseType(204)]
        [AllowAnonymous]
        public async Task<IActionResult> GetDisplayAuctionSession(string displayToken, int auctionSessionId)
        {
            var session = await _displayService.GetDisplayAuctionSession(displayToken, auctionSessionId);
            if (session != null)
                return Ok(_mapper.Map<AuctionSessionDto>(session));
            return NoContent();
        }

        /// <summary>
        /// Gets the latest bids by lot identifier.
        /// </summary>
        /// <param name="displayToken">The display token.</param>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDisplayBids/{displayToken}/{lotId}")]
        [ProducesResponseType(typeof(IEnumerable<BidDto>), 200)]
        [ProducesResponseType(204)]
        [AllowAnonymous]
        public async Task<IActionResult> GetDisplayBids(string displayToken, int lotId)
        {
            var bids = await _displayService.GetDisplayBids(displayToken, lotId);
            if (bids != null && bids.Any())
                return Ok(bids.Select(bid => _mapper.Map<BidDto>(bid)));
            return NoContent();
        }

        /// <summary>
        /// Gets the display media.
        /// </summary>
        /// <param name="displayToken">The display token.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDisplayMedia/{displayToken}")]
        [ProducesResponseType(typeof(IEnumerable<MediaDto>), 200)]
        [ProducesResponseType(204)]
        [AllowAnonymous]
        public async Task<IActionResult> GetDisplayMedia(string displayToken)
        {
            var medias = await _displayService.GetDisplayMedia(displayToken);
            if (medias != null && medias.Any())
                return Ok(medias.Select(media => _mapper.Map<MediaDto>(media)));
            return NoContent();
        }

        /// <summary>
        /// Gets the display access token.
        /// </summary>
        /// <param name="displayId">The display identifier.</param>
        /// <returns>access token for validating display</returns>
        [HttpGet]
        [Route("GetDisplayAccessToken/{displayId}")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetDisplayAccessToken(int displayId)
        {
            return Ok(_displayService.ProtectDisplayAccessToken(displayId));
        }

        /// <summary>
        /// Adds a new Display to the system
        /// </summary>
        /// <param name="Display">The Display object to be added</param>
        /// <returns>the created Display with the system generated identifier</returns>
        /// <response code="201">Returns the created Display</response>
        /// <response code="400">If the request was not valid</response>
        [HttpPost]
        [ProducesResponseType(typeof(DisplayDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddDisplay(DisplayDto Display)
        {
            var addTask = await _displayService.AddDisplay(Display);
            if (!addTask.IsCancelled)
                return Created("", _mapper.Map<DisplayDto>(addTask));
            return BadRequest("Process has been cancelled: " + addTask.CancellationReason);
        }

        /// <summary>
        /// Updates the Display.
        /// </summary>
        /// <param name="DisplayId">The unique ID of the Display to be updated</param>
        /// <param name="Display">The Display object containing the updated fields</param>
        /// <returns>the updated Display</returns>
        /// <response code="200">Returns the updated Display</response>
        /// <response code="400">If the new Display update information was not valid</response>
        [HttpPut("{DisplayId}")]
        [ProducesResponseType(typeof(DisplayDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateDisplay(int DisplayId, DisplayDto Display)
        {
            if (Display.ArePropertiesAllNull())
                return BadRequest("No valid Display field updates were received");
            
            var updateTask = await _displayService.UpdateDisplay(DisplayId, Display);
            if (!updateTask.IsCancelled)
                return Ok(_mapper.Map<DisplayDto>(updateTask));
            return BadRequest(updateTask.CancellationReason);            
        }

        /// <summary>
        /// Deletes the Display.
        /// </summary>
        /// <param name="displayId">The unique ID of the Display to be deleted</param>
        /// <returns>a deleted Display message</returns>
        /// <response code="200">Returns Display Deleted Message</response>
        /// <response code="400">If an issue occurred trying to delete the Display</response>
        [HttpDelete("{displayId}")]
        public async Task<IActionResult> DeleteDisplay(int displayId)
        {
            var deleteTask = await _displayService.DeleteDisplay(displayId);
            if (!deleteTask.IsCancelled)
                return Ok("Display Deleted");
            return BadRequest(deleteTask.CancellationReason);
        }
    }
}