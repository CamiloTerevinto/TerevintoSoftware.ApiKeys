using Microsoft.AspNetCore.Authentication;

namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys
{
    /// <summary>
    /// Contains the options needed by <see cref="ApiKeyAuthenticationHandler"/>
    /// </summary>
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the name of the header used for the API Key. 
        /// <para></para>
        /// If not set, the default value is "x-api-key".
        /// </summary>
        public static string HeaderName { get; set; } = "x-api-key";

        /// <summary>
        /// Gets or sets the name of the scheme used for authenticating API keys.
        /// <para></para>
        /// If not set, the default value is "ApiKey".
        /// </summary>
        public static string DefaultScheme { get; set; } = "ApiKey";

        /// <summary>
        /// Gets or sets the log configuration for the event when a request is received without a valid API Key header. 
        /// <para></para>
        /// A single parameter is expected for the header name. If null, no log is written.
        /// </summary>
        public (LogLevel, string)? NoApiKeyHeaderLog { get; set; } = (LogLevel.Information, "An API request was received without the {HeaderName} header.");

        /// <summary>
        /// Gets or sets the log configuration for the event when a request is received without a valid API Key header. 
        /// <para></para>
        /// A single parameter is expected for the API Key received. If null, no log is written.
        /// </summary>
        public (LogLevel, string)? InvalidApiKeyLog { get; set; } = (LogLevel.Warning, "An API request was received with an invalid API Key: {ApiKey}.");

        /// <summary>
        /// Gets or sets the name of the log scope used for the owner ID of the API Key. 
        /// <para></para>
        /// If not set, the default value is "{ApiKeyOwnerId}". If null, no log scope is used.
        /// </summary>
        public string? ApiKeyOwnerIdLogScopeName { get; set; } = "{ApiKeyOwnerId}";

        /// <summary>
        /// Gets or sets the log configuration for the event when a request is received with a valid API Key header. 
        /// <para></para>
        /// No parameter is expected. If null, no log is written.
        /// </summary>
        public (LogLevel, string)? ValidApiKeyLog { get; set; } = (LogLevel.Information, "Client authenticated.");

        /// <summary>
        /// Gets or sets the message used to return an error for an invalid API key (or lack thereof).
        /// <para></para>
        /// If not set, the default value is "Invalid API Key". Must not be null.
        /// </summary>
        public string FailureMessage { get; set; } = "Invalid API Key";
    }
}