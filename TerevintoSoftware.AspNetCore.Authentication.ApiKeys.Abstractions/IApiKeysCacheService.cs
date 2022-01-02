namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions
{
    /// <summary>
    /// Service used to resolve and invalidate API keys.
    /// </summary>
    public interface IApiKeysCacheService
    {
        /// <summary>
        /// Gets a API Key Owner ID (owner of the API key) from its API Key.
        /// </summary>
        /// <param name="apiKey">The API Key received on the HTTP request.</param>
        /// <returns>The ID of the owner of the API Key if the key was found, null otherwise.</returns>
        ValueTask<string?> GetOwnerIdFromApiKey(string apiKey);

        /// <summary>
        /// Invalidates (removes from cache and/or permanent storage) an API key.
        /// </summary>
        /// <param name="apiKey">The API Key to invalidate</param>
        /// <returns>A task representing the operation.</returns>
        Task InvalidateApiKey(string apiKey);
    }
}