using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Helpers;
using BT.Auctions.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace BT.Auctions.API.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Main venue API endpoint for all external communication with the auctions application
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenuesController"/> class.
        /// </summary>
        /// <param name="venueService">The venue service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">Automapper mapping object</param>
        public VenuesController(IVenueService venueService, ILogger<VenuesController> logger, IMapper mapper)
        {
            _mapper = mapper;
            _venueService = venueService;
        }

        /// <summary>
        /// Gets all venues currently in the system
        /// </summary>
        /// <returns>An array of venues</returns>
        /// <response code="200">Returns a list of venues</response>
        /// <response code="204">If no venues are found</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VenueDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetVenues()
        {
            var venues = await _venueService.GetVenues();
            if (venues != null && venues.Any())
                return Ok(venues.Select(venue => _mapper.Map<VenueDto>(venue)));
            return NoContent();
        }

        /// <summary>
        /// Gets the paged venues.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPagedVenues")]
        [ProducesResponseType(typeof(IEnumerable<Venue>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetPagedVenues(int pageNumber, int pageSize)
        {
            var venues = await _venueService.GetPagedVenues(pageNumber, pageSize);
            if (venues != null)
                return Ok(venues);
            return NoContent();
        }

        /// <summary>
        /// Gets the venue by identifier.
        /// </summary>
        /// <param name="id">The ID of the requested venue</param>
        /// <returns>The requested venue</returns>
        /// <response code="200">Returns a requested venue</response>
        /// <response code="400">If a venue by the requested ID is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VenueDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetVenueById(int id)
        {
            var venueTask = await _venueService.GetVenueById(id);
            if (!venueTask.IsCancelled)
                return Ok(_mapper.Map<VenueDto>(venueTask));
            return BadRequest(venueTask.CancellationReason);
        }

        /// <summary>
        /// Adds a new venue to the system
        /// </summary>
        /// <param name="venue">The venue object to be added</param>
        /// <returns>the created venue with the system generated identifier</returns>
        /// <response code="201">Returns the created venue</response>
        /// <response code="400">If the request was not valid</response>
        [HttpPost]
        [ProducesResponseType(typeof(VenueDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddVenue(VenueDto venue)
        {
            var addTask = await _venueService.AddVenue(venue);
            if (!addTask.IsCancelled)
                return Created("", _mapper.Map<VenueDto>(addTask));
            return BadRequest("Process has been cancelled: " + addTask.CancellationReason);
        }

        /// <summary>
        /// Updates the venue.
        /// </summary>
        /// <param name="id">The unique ID of the venue to be updated</param>
        /// <param name="venue">The venue object containing the updated fields</param>
        /// <returns>the updated venue</returns>
        /// <response code="200">Returns the updated venue</response>
        /// <response code="400">If the new venue update information was not valid</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(VenueDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateVenue(int id, VenueDto venue)
        {
            if(venue.ArePropertiesAllNull())
                return BadRequest("No valid venue field updates were received");
            
            var updateTask = await _venueService.UpdateVenue(id, venue);
            if (!updateTask.IsCancelled)
                return Ok(_mapper.Map<VenueDto>(updateTask));
            return BadRequest(updateTask.CancellationReason);            
        }

        /// <summary>
        /// Deletes the venue.
        /// </summary>
        /// <param name="id">The unique ID of the venue to be deleted</param>
        /// <returns>a deleted venue message</returns>
        /// <response code="200">Returns Venue Deleted Message</response>
        /// <response code="400">If an issue occurred trying to delete the Venue</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            var deleteTask = await _venueService.DeleteVenue(id);
            if (!deleteTask.IsCancelled)
                return Ok("Venue Deleted");
            return BadRequest(deleteTask.CancellationReason);
        }
    }
}
