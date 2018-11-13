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
    /// Main LotDetail API endpoint for all external communication with the auctions application
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LotDetailsController : ControllerBase
    {
        private readonly ILotDetailService _lotDetailService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LotDetailsController"/> class.
        /// </summary>
        /// <param name="lotDetailService">The LotDetail service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">Automapper mapping object</param>
        public LotDetailsController(ILotDetailService lotDetailService, ILogger<LotDetailsController> logger, IMapper mapper)
        {
            _mapper = mapper;
            _lotDetailService = lotDetailService;
        }

        /// <summary>
        /// Gets the LotDetail by identifier.
        /// </summary>
        /// <param name="lotId">The ID of the lot that will be searched</param>
        /// <returns>The requested LotDetail</returns>
        /// <response code="200">Returns a requested LotDetails</response>
        /// <response code="204">If no lotDetails are found for auction session</response>
        [HttpGet("{lotId}")]
        [ProducesResponseType(typeof(IEnumerable<LotDetail>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetLotDetailByLotId(int lotId)
        {
            var lotDetail = await _lotDetailService.GetLotDetailByLotId(lotId);
            if (lotDetail != null && lotDetail.Any())
                return Ok(lotDetail.Select(info => _mapper.Map<LotDetailDto>(info)));
            return NoContent();
        }

        /// <summary>
        /// Adds a new LotDetail to the system
        /// </summary>
        /// <param name="lotDetail">The LotDetail object to be added</param>
        /// <returns>the created LotDetail with the system generated identifier</returns>
        /// <response code="201">Returns the created LotDetail</response>
        /// <response code="400">If the request was not valid</response>
        [HttpPost]
        [ProducesResponseType(typeof(LotDetail), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddLotDetail(LotDetailDto lotDetail)
        {
            var addTask = await _lotDetailService.AddLotDetail(lotDetail);
            if (!addTask.IsCancelled)
                return Created("", _mapper.Map<LotDetailDto>(addTask));
            return BadRequest("Process has been cancelled: " + addTask.CancellationReason);
        }

        /// <summary>
        /// Updates the LotDetail.
        /// </summary>
        /// <param name="lotDetailId">The unique ID of the LotDetail to be updated</param>
        /// <param name="lotDetail">The LotDetail object containing the updated fields</param>
        /// <returns>the updated LotDetail</returns>
        /// <response code="200">Returns the updated LotDetail</response>
        /// <response code="400">If the new LotDetail update information was not valid</response>
        [HttpPut("{lotDetailId}")]
        [ProducesResponseType(typeof(LotDetail), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateLotDetail(int lotDetailId, LotDetailDto lotDetail)
        {
            if(lotDetail.ArePropertiesAllNull())
                return BadRequest("No valid LotDetail field updates were received");
            
            var updateTask = await _lotDetailService.UpdateLotDetail(lotDetailId, lotDetail);
            if (!updateTask.IsCancelled)
                return Ok(_mapper.Map<LotDetailDto>(updateTask));
            return BadRequest(updateTask.CancellationReason);            
        }

        /// <summary>
        /// Deletes the Lot Detail.
        /// </summary>
        /// <param name="lotDetailId">The lot detail identifier.</param>
        /// <returns>
        /// a deleted Lot Detail message
        /// </returns>
        /// <response code="200">Returns Lot Detail Deleted Message</response>
        /// <response code="400">If an issue occurred trying to delete the LotDetail</response>
        [HttpDelete("{lotDetailId}")]
        public async Task<IActionResult> DeleteLotDetail(int lotDetailId)
        {
            var deleteTask = await _lotDetailService.DeleteLotDetail(lotDetailId);
            if (!deleteTask.IsCancelled)
                return Ok("Lot Detail Deleted");
            return BadRequest(deleteTask.CancellationReason);
        }
    }
}
