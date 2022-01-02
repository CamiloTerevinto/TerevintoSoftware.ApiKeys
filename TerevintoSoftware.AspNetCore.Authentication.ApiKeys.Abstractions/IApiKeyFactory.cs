namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions
{
    /// <summary>
    /// Factory for creating API Keys.
    /// </summary>
    public interface IApiKeyFactory
    {
        /// <summary>
        /// Generates an API Key.
        /// </summary>
        /// <returns>The API Key generated.</returns>
        string GenerateApiKey();
    }
}
