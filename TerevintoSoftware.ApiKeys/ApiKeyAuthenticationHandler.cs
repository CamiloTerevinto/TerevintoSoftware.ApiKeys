using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.Encodings.Web;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions;

namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys;

/// <summary>
/// Authentication handler for API Keys. 
/// Validates the API Key header format and that an API key can be found by <see cref="IApiKeysCacheService"/>.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeysCacheService _cacheService;
    private readonly IClaimsPrincipalFactory _claimsPrincipalFactory;
    private readonly IStringLocalizerFactory _stringLocalizerFactory;

    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> optionsMonitor, ILoggerFactory loggerFactory, UrlEncoder encoder,
        ISystemClock clock, IApiKeysCacheService cacheService, IClaimsPrincipalFactory claimsPrincipalFactory,
        IStringLocalizerFactory stringLocalizerFactory) : base(optionsMonitor, loggerFactory, encoder, clock)
    {
        _cacheService = cacheService;
        _claimsPrincipalFactory = claimsPrincipalFactory;
        _stringLocalizerFactory = stringLocalizerFactory;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var apiKey) || apiKey.Count != 1)
        {
            if (Options.NoApiKeyHeaderLog is { } noApiKeyHeaderLog)
            {
                Logger.Log(noApiKeyHeaderLog.Item1, noApiKeyHeaderLog.Item2, ApiKeyAuthenticationOptions.HeaderName);
            }

            return AuthenticateResult.Fail(Options.FailureMessage);
        }

        var apiKeyOwnerId = await _cacheService.GetOwnerIdFromApiKey(apiKey);

        if (string.IsNullOrWhiteSpace(apiKeyOwnerId))
        {
            if (Options.InvalidApiKeyLog is { } invalidApiKeyLog)
            {
                Logger.Log(invalidApiKeyLog.Item1, invalidApiKeyLog.Item2, apiKey);
            }

            return AuthenticateResult.Fail(Options.FailureMessage);
        }

        if (!string.IsNullOrEmpty(Options.ApiKeyOwnerIdLogScopeName))
        {
            Logger.BeginScope(Options.ApiKeyOwnerIdLogScopeName, apiKeyOwnerId);
        }

        if (Options.ValidApiKeyLog is { } validApiKeyLog)
        {
            Logger.Log(validApiKeyLog.Item1, validApiKeyLog.Item2);
        }

        var principal = await _claimsPrincipalFactory.CreateClaimsPrincipal(apiKeyOwnerId);
        var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.DefaultScheme);

        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var localizer = _stringLocalizerFactory.Create(nameof(ApiKeyAuthenticationHandler), new AssemblyName(Assembly.GetEntryAssembly()!.FullName!).Name!);
        var failureMessage = localizer[Options.FailureMessage];

        Response.StatusCode = StatusCodes.Status401Unauthorized;

        await Response.WriteAsJsonAsync(new ProblemDetails
        {
            Detail = failureMessage,
            Status = StatusCodes.Status401Unauthorized,
            Title = failureMessage
        });
    }
}
