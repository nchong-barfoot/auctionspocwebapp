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
using BT.Auctions.API.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace BT.Auctions.API.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Main Image API endpoint for all external communication with the auctions application
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagesController"/> class.
        /// </summary>
        /// <param name="imageService">The Image service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">Automapper mapping object</param>
        public ImagesController(IImageService imageService, ILogger<ImagesController> logger, IMapper mapper)
        {
            _mapper = mapper;
            _imageService = imageService;
        }

        /// <summary>
        /// Gets all images currently in the system
        /// </summary>
        /// <returns>An array of images</returns>
        /// <response code="200">Returns a list of images</response>
        /// <response code="204">If no images are found</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ImageDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetImages()
        {
            var images = await _imageService.GetImages();
            if (images != null && images.Any())
                return Ok(images.Select(image => _mapper.Map<ImageDto>(image)));
            return NoContent();
        }

        /// <summary>
        /// Gets the Image by identifier.
        /// </summary>
        /// <param name="lotId">The ID of the Lot that will be searched</param>
        /// <returns>
        /// The requested Image
        /// </returns>
        /// <response code="200">Returns a requested Images</response>
        /// <response code="204">If no images are found for lot</response>
        [HttpGet("{lotId}")]
        [ProducesResponseType(typeof(IEnumerable<ImageDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetImageByLotId(int lotId)
        {
            var images = await _imageService.GetImagesByLotId(lotId);
            if (images != null && images.Any())
                return Ok(images.Select(image => _mapper.Map<ImageDto>(image)));
            return NoContent();
        }

        /// <summary>
        /// Adds a new Image to the system
        /// </summary>
        /// <param name="image">The Image object to be added</param>
        /// <returns>the created Image with the system generated identifier</returns>
        /// <response code="201">Returns the created Image</response>
        /// <response code="400">If the request was not valid</response>
        [HttpPost]
        [AllowAnonymous]
        [ServiceFilter(typeof(IpFilterAttribute))]
        [ProducesResponseType(typeof(ImageDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddImage(ImageDto image)
        {
            var addTask = await _imageService.AddImage(image);
            if (!addTask.IsCancelled)
                return Created("", _mapper.Map<ImageDto>(addTask));
            return BadRequest("Process has been cancelled: " + addTask.CancellationReason);
        }

        /// <summary>
        /// Updates the Image.
        /// </summary>
        /// <param name="imageId">The unique ID of the Image to be updated</param>
        /// <param name="image">The Image object containing the updated fields</param>
        /// <returns>the updated Image</returns>
        /// <response code="200">Returns the updated Image</response>
        /// <response code="400">If the new Image update information was not valid</response>
        [HttpPut("{imageId}")]
        [ProducesResponseType(typeof(Image), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateImage(int imageId, ImageDto image)
        {
            if (image.ArePropertiesAllNull())
                return BadRequest("No valid Image field updates were received");
            
            var updateTask = await _imageService.UpdateImage(imageId, image);
            if (!updateTask.IsCancelled)
                return Ok(_mapper.Map<ImageDto>(updateTask));
            return BadRequest(updateTask.CancellationReason);            
        }

        /// <summary>
        /// Deletes the venue.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <returns>
        /// a deleted venue message
        /// </returns>
        /// <response code="200">Returns Venue Deleted Message</response>
        /// <response code="400">If an issue occurred trying to delete the Venue</response>
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var deleteTask = await _imageService.DeleteImage(imageId);
            if (!deleteTask.IsCancelled)
                return Ok("Image Deleted");
            return BadRequest(deleteTask.CancellationReason);
        }
    }
}
