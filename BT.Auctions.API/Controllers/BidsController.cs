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
    /// Main Bid API endpoint for all external communication with the auctions application
    /// </summary>
    /// <seealso cref="T:Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BidsController : ControllerBase
    {
        private readonly IBidService _bidService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BidsController"/> class.
        /// </summary>
        /// <param name="bidService">The Bid service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">Automapper mapping object</param>
        public BidsController(IBidService bidService, ILogger<BidsController> logger, IMapper mapper)
        {
            _mapper = mapper;
            _bidService = bidService;
        }

        /// <summary>
        /// Gets the bids by lot identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        [HttpGet("{lotId}")]
        [ProducesResponseType(typeof(IEnumerable<BidDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetBidsByLotId(int lotId)
        {
            var bids = await _bidService.GetBidsByLotId(lotId);
            if (bids != null && bids.Any())
                return Ok(bids.Select(bid => _mapper.Map<BidDto>(bid)));
            return NoContent();
        }

        /// <summary>
        /// Gets the latest bids by lot identifier.
        /// </summary>
        /// <param name="lotId">The lot identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetLatestBidsByLotId/{lotId}")]
        [ProducesResponseType(typeof(IEnumerable<BidDto>), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> GetLatestBidsByLotId(int lotId)
        {
            var bids = await _bidService.GetLatestBidsByLotId(lotId);
            if (bids != null && bids.Any())
                return Ok(bids.Select(bid => _mapper.Map<BidDto>(bid)));
            return NoContent();
        }

        /// <summary>
        /// Adds a new Bid to the system
        /// </summary>
        /// <param name="bid">The Bid object to be added</param>
        /// <returns>the created Bid with the system generated identifier</returns>
        /// <response code="201">Returns the created Bid</response>
        /// <response code="400">If the request was not valid</response>
        [HttpPost]
        [ProducesResponseType(typeof(BidDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddBid(BidDto bid)
        {
            var addTask = await _bidService.AddBid(bid);
            if (!addTask.IsCancelled)
                return Created("", _mapper.Map<BidDto>(addTask));
            return BadRequest("Process has been cancelled: " + addTask.CancellationReason);
        }

        /// <summary>
        /// Updates the Bid.
        /// </summary>
        /// <param name="bidId">The unique ID of the Bid to be updated</param>
        /// <param name="bid">The Bid object containing the updated fields</param>
        /// <returns>the updated Bid</returns>
        /// <response code="200">Returns the updated Bid</response>
        /// <response code="400">If the new Bid update information was not valid</response>
        [HttpPut("{bidId}")]
        [ProducesResponseType(typeof(Bid), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateBid(int bidId, BidDto bid)
        {
            if (bid.ArePropertiesAllNull())
                return BadRequest("No valid Bid field updates were received");
            
            var updateTask = await _bidService.UpdateBid(bidId, bid);
            if (!updateTask.IsCancelled)
                return Ok(_mapper.Map<BidDto>(updateTask));
            return BadRequest(updateTask.CancellationReason);            
        }
    }
}
