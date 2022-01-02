using System.Security.Claims;

namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions
{
    /// <summary>
    /// Service used to generate <see cref="ClaimsPrincipal"/>s.
    /// </summary>
    public interface IClaimsPrincipalFactory
    {
        /// <summary>
        /// Creates a claims principal to use for authenticating a request.
        /// </summary>
        /// <param name="apiKeyOwnerId">The ID of the owner of the API Key.</param>
        /// <returns>A <see cref="ClaimsPrincipal"/> that represents the entity that initiated the HTTP request.</returns>
        Task<ClaimsPrincipal> CreateClaimsPrincipal(string apiKeyOwnerId);
    }
}
