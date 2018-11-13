using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BT.Auctions.API.Controllers
{
    /// <summary>
    /// Account Verification Controller
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <inheritdoc />
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsController" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public AccountsController(ILogger<AccountsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validation of a JWT being used to access the API
        /// </summary>
        /// <returns>
        /// Success if JWT is valid
        /// </returns>
        /// <response code="200">Returns a 200 if authorization passes</response>
        /// <response code="401">Failure to authorize</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult ValidateAccount()
        {
            //Log Access if required
            return Ok();
        }
    }
}
