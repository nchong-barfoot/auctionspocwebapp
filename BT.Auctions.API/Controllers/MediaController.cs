using AutoMapper;
using BT.Auctions.API.Models;
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
    /// Main Media API endpoint for all external communication with the auctions application
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MediasController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediasController"/> class.
        /// </summary>
        /// <param name="mediaService">The Media service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">Automapper mapping object</param>
        public MediasController(IMediaService mediaService, ILogger<MediasController> logger, IMapper mapper)
        {
            _mapper = mapper;
            _mediaService = mediaService;
        }

        /// <summary>
        /// Gets the Medias
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MediaDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetMedia()
        {
            var medias = await _mediaService.GetMedia();
            if (medias != null && medias.Any())
                return Ok(medias.Select(m => _mapper.Map<MediaDto>(m)));
            return NoContent();
        }

        /// <summary>
        /// Adds a new Media to the system
        /// </summary>
        /// <param name="media">The media.</param>
        /// <returns>
        /// the created Media with the system generated identifier
        /// </returns>
        /// <response code="201">Returns the created Media</response>
        /// <response code="400">If the request was not valid</response>
        [HttpPost]
        [ProducesResponseType(typeof(MediaDto), 201)]
        [ProducesResponseType(400)]
        [RequestSizeLimit(100000000)]
        public async Task<IActionResult> AddMedia([FromForm]MediaDto media)
        {
            if (media.Data == null || string.IsNullOrEmpty(media.Title))
            {
                return BadRequest("Media data and title are required");
            }
            var addTask = await _mediaService.AddMedia(media);
            if (!addTask.IsCancelled)
                return Created("", _mapper.Map<MediaDto>(addTask));
            return BadRequest("Process has been cancelled: " + addTask.CancellationReason);
        }

        /// <summary>
        /// Updates the Media.
        /// </summary>
        /// <param name="mediaId">The unique ID of the Media to be updated</param>
        /// <param name="media">The Media object containing the updated fields</param>
        /// <returns>the updated Media</returns>
        /// <response code="200">Returns the updated Media</response>
        /// <response code="400">If the new Media update information was not valid</response>
        [HttpPut("{mediaId}")]
        [ProducesResponseType(typeof(Media), 200)]
        [ProducesResponseType(400)]
        [RequestSizeLimit(100000000)]
        public async Task<IActionResult> UpdateMedia(int mediaId, [FromForm]MediaDto media)
        {
            if (media.ArePropertiesAllNull())
                return BadRequest("No valid Media field updates were received");
            
            var updateTask = await _mediaService.UpdateMedia(mediaId, media);
            if (!updateTask.IsCancelled)
                return Ok(_mapper.Map<MediaDto>(updateTask));
            return BadRequest(updateTask.CancellationReason);            
        }


        /// <summary>
        /// Deletes the media.
        /// </summary>
        /// <param name="mediaId">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{mediaId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteMedia(int mediaId)
        {
            var deleteTask = await _mediaService.DeleteMedia(mediaId);
            if (!deleteTask.IsCancelled)
                return Ok("Media Deleted");
            return BadRequest(deleteTask.CancellationReason);
        }
    }
}
