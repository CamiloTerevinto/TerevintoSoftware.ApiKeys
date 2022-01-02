using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions;

namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys
{
    /// <summary>
    /// Authentication handler for API Keys. 
    /// Validates the API Key header format and that an API key can be found by <see cref="IApiKeysCacheService"/>.
    /// </summary>
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IApiKeysCacheService _cacheService;
        private readonly IClaimsPrincipalFactory _claimsPrincipalFactory;
        private readonly ApiKeyAuthenticationOptions _options;

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> optionsMonitor, ILoggerFactory loggerFactory, UrlEncoder encoder,
            ISystemClock clock, IApiKeysCacheService cacheService, IClaimsPrincipalFactory claimsPrincipalFactory) : base(optionsMonitor, loggerFactory, encoder, clock)
        {
            _cacheService = cacheService;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _options = optionsMonitor.CurrentValue;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var apiKey) || apiKey.Count != 1)
            {
                if (_options.NoApiKeyHeaderLog is { } noApiKeyHeaderLog)
                {
                    Logger.Log(noApiKeyHeaderLog.Item1, noApiKeyHeaderLog.Item2, ApiKeyAuthenticationOptions.HeaderName);
                }

                return AuthenticateResult.Fail(_options.FailureMessage);
            }

            var apiKeyOwnerId = await _cacheService.GetOwnerIdFromApiKey(apiKey);

            if (string.IsNullOrWhiteSpace(apiKeyOwnerId))
            {
                if (_options.InvalidApiKeyLog is { } invalidApiKeyLog)
                {
                    Logger.Log(invalidApiKeyLog.Item1, invalidApiKeyLog.Item2, apiKey);
                }

                return AuthenticateResult.Fail(_options.FailureMessage);
            }

            if (!string.IsNullOrEmpty(_options.ApiKeyOwnerIdLogScopeName))
            {
                Logger.BeginScope(_options.ApiKeyOwnerIdLogScopeName, apiKeyOwnerId);
            }

            if (_options.ValidApiKeyLog is { } validApiKeyLog)
            {
                Logger.Log(validApiKeyLog.Item1, validApiKeyLog.Item2);
            }

            var principal = await _claimsPrincipalFactory.CreateClaimsPrincipal(apiKeyOwnerId);
            var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.DefaultScheme);

            return AuthenticateResult.Success(ticket);
        }
    }
}
