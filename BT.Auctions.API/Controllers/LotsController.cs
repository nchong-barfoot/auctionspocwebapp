using AutoMapper;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using BT.Auctions.API.Models.Helpers;
using BT.Auctions.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BT.Auctions.API.Helpers;

namespace BT.Auctions.API.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Main Lot API endpoint for all external communication with the auctions application
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LotsController : ControllerBase
    {
        private readonly ILotService _lotService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LotsController"/> class.
        /// </summary>
        /// <param name="lotService">The Lot service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">Automapper mapping object</param>
        public LotsController(ILotService lotService, ILogger<LotsController> logger, IMapper mapper)
        {
            _mapper = mapper;
            _lotService = lotService;
        }

        /// <summary>
        /// Gets all lots currently in the system
        /// </summary>
        /// <returns>An array of lots</returns>
        /// <response code="200">Returns a list of lots</response>
        /// <response code="204">If no lots are found</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LotDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetLots()
        {
            var lots = await _lotService.GetLots();
            if (lots != null && lots.Any())
                return Ok(lots.Select(lot => _mapper.Map<LotDto>(lot)));
            return NoContent();
        }

        /// <summary>
        /// Gets the Lot by identifier.
        /// </summary>
        /// <param name="auctionSessionId">The ID of the auction session ID that will be searched</param>
        /// <returns>
        /// The requested Lot
        /// </returns>
        /// <response code="200">Returns a requested Lots</response>
        /// <response code="204">If no lots are found for auction session</response>
        [HttpGet("{auctionSessionId}")]
        [ProducesResponseType(typeof(IEnumerable<LotDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetLotsByAuctionSessionId(int auctionSessionId)
        {
            var lots = await _lotService.GetLotsByAuctionSessionId(auctionSessionId);
            if (lots != null && lots.Any())
                return Ok(lots.Select(lot => _mapper.Map<LotDto>(lot)));
            return NoContent();
        }

        /// <summary>
        /// Adds a new Lot to the system
        /// </summary>
        /// <param name="lot">The Lot object to be added</param>
        /// <returns>the created Lot with the system generated identifier</returns>
        /// <response code="201">Returns the created Lot</response>
        /// <response code="400">If the request was not valid</response>
        [HttpPost]
        [AllowAnonymous]
        [ServiceFilter(typeof(IpFilterAttribute))]
        [ProducesResponseType(typeof(LotDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddLot(LotDto lot)
        {
            var addTask = await _lotService.AddLot(lot);
            if (!addTask.IsCancelled)
                return Created("", _mapper.Map<LotDto>(addTask));
            return BadRequest("Process has been cancelled: " + addTask.CancellationReason);
        }

        /// <summary>
        /// Updates the Lot.
        /// </summary>
        /// <param name="lotId">The unique ID of the Lot to be updated</param>
        /// <param name="lot">The Lot object containing the updated fields</param>
        /// <returns>the updated Lot</returns>
        /// <response code="200">Returns the updated Lot</response>
        /// <response code="400">If the new Lot update information was not valid</response>
        [HttpPut("{lotId}")]
        [ProducesResponseType(typeof(Lot), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateLot(int lotId, LotDto lot)
        {
            if (lot.ArePropertiesAllNull())
                return BadRequest("No valid Lot field updates were received");
            
            var updateTask = await _lotService.UpdateLot(lotId, lot);
            if (!updateTask.IsCancelled)
                return Ok(_mapper.Map<LotDto>(updateTask));
            return BadRequest(updateTask.CancellationReason);
        }
    }
}
