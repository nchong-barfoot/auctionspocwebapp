using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using BT.Auctions.API.Models;
using Microsoft.Extensions.Options;

namespace BT.Auctions.API.Helpers
{
    /// <summary>
    /// Used to filter access to the api
    /// </summary>
    public class IpFilterAttribute : ActionFilterAttribute
    {
        private readonly IOptions<ServiceSettings> _serviceSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="IpFilterAttribute"/> class.
        /// </summary>
        /// <param name="serviceSettings">The service settings.</param>
        public IpFilterAttribute(IOptions<ServiceSettings> serviceSettings)
        {
            _serviceSettings = serviceSettings;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress;

            if (!IsIpAddressAllowed(ipAddress.ToString().Trim()))
            {
                throw new UnauthorizedAccessException($"Unauthorized Access from IP: {ipAddress}");
            }

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Determines whether the ip address provided is allowed.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns>
        ///   <c>true</c> if the ip address provided is allowed; otherwise, <c>false</c>.
        /// </returns>
        private bool IsIpAddressAllowed(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress) || string.IsNullOrEmpty(_serviceSettings.Value.AllowedPostIPAddresses)) return false;

            //AllowedPostIPAddresses are managed in the Azure KeyVault
            var addresses = _serviceSettings.Value.AllowedPostIPAddresses.Split(';');
            return addresses.Any(a => a.Trim().Equals(ipAddress, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
