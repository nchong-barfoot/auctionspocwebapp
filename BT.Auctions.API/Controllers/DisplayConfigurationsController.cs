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
    /// Main Display Configuration API endpoint for all external communication with the auctions application
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DisplayConfigurationsController : ControllerBase
    {
        private readonly IDisplayConfigurationService _displayConfigurationService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayConfigurationsController"/> class.
        /// </summary>
        /// <param name="displayConfigurationService">The DisplayConfiguration service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">Automapper mapping object</param>
        public DisplayConfigurationsController(IDisplayConfigurationService displayConfigurationService, ILogger<DisplayConfigurationsController> logger, IMapper mapper)
        {
            _mapper = mapper;
            _displayConfigurationService = displayConfigurationService;
        }

        /// <summary>
        /// Gets the DisplayConfiguration by identifier.
        /// </summary>
        /// <returns>The requested DisplayConfiguration</returns>
        /// <response code="200">Returns a requested DisplayConfigurations</response>
        /// <response code="204">If no DisplayConfigurations are found for the venue</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DisplayConfigurationDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetDisplayConfigurationByVenueId()
        {
            var displayConfigurations = await _displayConfigurationService.GetDisplayConfigurations();
            if (displayConfigurations != null && displayConfigurations.Any())
                return Ok(displayConfigurations.Select(config => _mapper.Map<DisplayConfigurationDto>(config)));
            return NoContent();
        }

        /// <summary>
        /// Gets the display configuration by identifier.
        /// </summary>
        /// <param name="displayGroupId">The display group identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDisplayGroupConfigurationsByDisplayGroupId/{displayGroupId}")]
        [ProducesResponseType(typeof(IEnumerable<DisplayConfigurationDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetDisplayGroupConfigurationsByDisplayGroupId(int displayGroupId)
        {
            var displayConfiguration = await _displayConfigurationService.GetDisplayGroupConfigurationsByDisplayGroupId(displayGroupId);
            if (displayConfiguration != null)
                return Ok(displayConfiguration);
            return NoContent();
        }

        /// <summary>
        /// Adds a new DisplayConfiguration to the system
        /// </summary>
        /// <param name="displayConfiguration">The DisplayConfiguration object to be added</param>
        /// <returns>the created DisplayConfiguration with the system generated identifier</returns>
        /// <response code="201">Returns the created DisplayConfiguration</response>
        /// <response code="400">If the request was not valid</response>
        [HttpPost]
        [ProducesResponseType(typeof(DisplayConfigurationDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddDisplayConfiguration(DisplayConfigurationDto displayConfiguration)
        {
            var addTask = await _displayConfigurationService.AddDisplayConfiguration(displayConfiguration);
            if (!addTask.IsCancelled)
                return Created("", _mapper.Map<DisplayConfigurationDto>(addTask));
            return BadRequest("Process has been cancelled: " + addTask.CancellationReason);
        }

        /// <summary>
        /// Updates the DisplayConfiguration.
        /// </summary>
        /// <param name="displayConfigurationId">The unique ID of the DisplayConfiguration to be updated</param>
        /// <param name="displayConfiguration">The DisplayConfiguration object containing the updated fields</param>
        /// <returns>the updated DisplayConfiguration</returns>
        /// <response code="200">Returns the updated DisplayConfiguration</response>
        /// <response code="400">If the new DisplayConfiguration update information was not valid</response>
        [HttpPut("{displayConfigurationId}")]
        [ProducesResponseType(typeof(DisplayConfigurationDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateDisplayConfiguration(int displayConfigurationId, DisplayConfigurationDto displayConfiguration)
        {
            if (displayConfiguration.ArePropertiesAllNull())
                return BadRequest("No valid DisplayConfiguration field updates were received");

            var updateTask = await _displayConfigurationService.UpdateDisplayConfiguration(displayConfigurationId, displayConfiguration);
            if (!updateTask.IsCancelled)
                return Ok(_mapper.Map<DisplayConfigurationDto>(updateTask));
            return BadRequest(updateTask.CancellationReason);            
        }

        /// <summary>
        /// Deletes the Display Configuration.
        /// </summary>
        /// <param name="displayConfigurationId">The display configuration identifier.</param>
        /// <returns>
        /// a deleted Display Configuration message
        /// </returns>
        /// <response code="200">Returns Display Configuration Deleted Message</response>
        /// <response code="400">If an issue occurred trying to delete the DisplayGroup</response>
        [HttpDelete("{displayConfigurationId}")]
        public async Task<IActionResult> DeleteDisplayConfiguration(int displayConfigurationId)
        {
            var deleteTask = await _displayConfigurationService.DeleteDisplayConfiguration(displayConfigurationId);
            if (!deleteTask.IsCancelled)
                return Ok("Display Configuration Deleted");
            return BadRequest(deleteTask.CancellationReason);
        }
    }
}