using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BT.Auctions.API.Helpers
{
    /// <inheritdoc />
    /// <summary>
    /// Class to validate the google id_token passed front the front end login.
    /// Should be used for validation before allowing api access
    /// </summary>
    /// <seealso cref="T:Microsoft.IdentityModel.Tokens.ISecurityTokenValidator" />
    public class GoogleTokenValidator : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IEnumerable<string> _clientAudience;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleTokenValidator"/> class.
        /// </summary>
        public GoogleTokenValidator(IEnumerable<string> clientAudience)
        {
            _tokenHandler = new JwtSecurityTokenHandler();
            _clientAudience = clientAudience;
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns true if a token can be validated.
        /// </summary>
        public bool CanValidateToken => true;

        /// <inheritdoc />
        /// <summary>
        /// Gets and sets the maximum size in bytes, that a will be processed.
        /// </summary>
        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        /// <inheritdoc />
        /// <summary>
        /// Returns true if the token can be read, false otherwise.
        /// </summary>
        /// <param name="securityToken"></param>
        /// <returns></returns>
        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        /// <inheritdoc />
        /// <summary>
        /// Validates a token passed as a string using <see cref="T:Microsoft.IdentityModel.Tokens.TokenValidationParameters" />
        /// Google certificates are cached, and refreshed once per hour.This can be overridden by setting ForceGoogleCertRefresh to true.
        /// </summary>
        /// <param name="securityToken"></param>
        /// <param name="validationParameters"></param>
        /// <param name="validatedToken"></param>
        /// <returns></returns>
        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            //https://developers.google.com/identity/protocols/OpenIDConnect#validatinganidtoken
            validatedToken = null;
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                HostedDomain = "barfoot.co.nz",
                Audience = _clientAudience
            };

            var payload = GoogleJsonWebSignature.ValidateAsync(securityToken, settings).Result;

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Email, payload.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, payload.Subject),
                    new Claim(JwtRegisteredClaimNames.Iss, payload.Issuer)
                };

            try
            {
                var principle = new ClaimsPrincipal();
                principle.AddIdentity(new ClaimsIdentity(claims, "Google"));
                return principle;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;

            }
        }
    }
}
