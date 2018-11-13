using AutoMapper;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Helpers;
using BT.Auctions.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BT.Auctions.API.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Main DisplayGroup API endpoint for all external communication with the auctions application
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DisplayGroupsController : ControllerBase
    {
        private readonly IDisplayGroupService _displayService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayGroupsController"/> class.
        /// </summary>
        /// <param name="displayService">The DisplayGroup service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">Automapper mapping object</param>
        public DisplayGroupsController(IDisplayGroupService displayService, ILogger<DisplayGroupsController> logger, IMapper mapper)
        {
            _mapper = mapper;
            _displayService = displayService;
        }

        /// <summary>
        /// Gets all Display Groups
        /// </summary>
        /// <returns>List of DisplayGroups</returns>
        /// <response code="200">Returns a requested DisplayGroups</response>
        /// <response code="204">If no DisplayGroups are found</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DisplayGroupDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetDisplayGroups()
        {
            var displayGroups = await _displayService.GetDisplayGroups();
            if (displayGroups != null && displayGroups.Any())
                return Ok(displayGroups.Select(displayGroup => _mapper.Map<DisplayGroupDto>(displayGroup)));
            return NoContent();
        }

        /// <summary>
        /// Gets the paged display groups.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="auctionSessionId">The auction session identifier.</param>
        /// <param name="venueId">The venue identifier.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPagedDisplayGroups")]
        [ProducesResponseType(typeof(IEnumerable<DisplayGroupDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetPagedDisplayGroups(int pageNumber, int pageSize, int? auctionSessionId, int? venueId, string timeZone)
        {
            var displayGroups = await _displayService.GetPagedDisplayGroups(pageNumber, pageSize, auctionSessionId, venueId, timeZone);
            if (displayGroups != null)
                return Ok(displayGroups);
            return NoContent();
        }

        /// <summary>
        /// Adds a new DisplayGroup to the system
        /// </summary>
        /// <param name="DisplayGroup">The DisplayGroup object to be added</param>
        /// <returns>the created DisplayGroup with the system generated identifier</returns>
        /// <response code="201">Returns the created DisplayGroup</response>
        /// <response code="400">If the request was not valid</response>
        [HttpPost]
        [ProducesResponseType(typeof(DisplayGroupDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddDisplayGroup(DisplayGroupDto DisplayGroup)
        {
            var addTask = await _displayService.AddDisplayGroup(DisplayGroup);
            if (!addTask.IsCancelled)
                return Created("", _mapper.Map<DisplayGroupDto>(addTask));
            return BadRequest("Process has been cancelled: " + addTask.CancellationReason);
        }

        /// <summary>
        /// Updates the DisplayGroup.
        /// </summary>
        /// <param name="DisplayGroupId">The unique ID of the DisplayGroup to be updated</param>
        /// <param name="DisplayGroup">The DisplayGroup object containing the updated fields</param>
        /// <returns>the updated DisplayGroup</returns>
        /// <response code="200">Returns the updated DisplayGroup</response>
        /// <response code="400">If the new DisplayGroup update information was not valid</response>
        [HttpPut("{DisplayGroupId}")]
        [ProducesResponseType(typeof(DisplayGroupDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateDisplayGroup(int DisplayGroupId, DisplayGroupDto DisplayGroup)
        {
            if (DisplayGroup.ArePropertiesAllNull())
                return BadRequest("No valid DisplayGroup field updates were received");
            
            var updateTask = await _displayService.UpdateDisplayGroup(DisplayGroupId, DisplayGroup);
            if (!updateTask.IsCancelled)
                return Ok(_mapper.Map<DisplayGroupDto>(updateTask));
            return BadRequest(updateTask.CancellationReason);            
        }

        /// <summary>
        /// Deletes the DisplayGroup.
        /// </summary>
        /// <param name="displayId">The unique ID of the DisplayGroup to be deleted</param>
        /// <returns>a deleted DisplayGroup message</returns>
        /// <response code="200">Returns DisplayGroup Deleted Message</response>
        /// <response code="400">If an issue occurred trying to delete the DisplayGroup</response>
        [HttpDelete("{displayId}")]
        public async Task<IActionResult> DeleteVenue(int displayId)
        {
            var deleteTask = await _displayService.DeleteDisplayGroup(displayId);
            if (!deleteTask.IsCancelled)
                return Ok("DisplayGroup Deleted");
            return BadRequest(deleteTask.CancellationReason);
        }
    }
}