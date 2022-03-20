﻿using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.CodeAnalysis;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds API Keys authentication to the ASP.NET Core middleware.
        /// </summary>
        /// <param name="services">The container to add the services to.</param>
        /// <param name="configureOptions">Used to configure scheme options.</param>
        /// <param name="useAsDefaultScheme">Whether to set API Keys as the default authentication scheme.</param>
        /// <returns>The service container for further chaining.</returns>
        public static IServiceCollection AddApiKeys(this IServiceCollection services, Action<ApiKeyAuthenticationOptions> configureOptions, bool useAsDefaultScheme)
        {
            var authenticationBuilder = useAsDefaultScheme ? services.AddAuthentication(ApiKeyAuthenticationOptions.DefaultScheme) : services.AddAuthentication();

            authenticationBuilder
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, configureOptions);

            return services;
        }

        /// <summary>
        /// Adds API Keys authentication to the ASP.NET Core middleware.
        /// </summary>
        /// <param name="authenticationBuilder">The authentication builder retrieved by calling <see cref="AuthenticationServiceCollectionExtensions.AddAuthentication"/></param>
        /// <param name="configureOptions">Used to configure scheme options.</param>
        /// <returns>The authentication builder for further chaining.</returns>
        public static AuthenticationBuilder AddApiKeys(this AuthenticationBuilder authenticationBuilder, Action<ApiKeyAuthenticationOptions> configureOptions)
        {
            return authenticationBuilder
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, configureOptions);
        }

        /// <summary>
        /// Adds the default API Key generator, see: <seealso cref="DefaultApiKeyFactory"/>.
        /// </summary>
        /// <param name="services">The container to add the services to.</param>
        /// <param name="apiKeyGenerationOptions">The options used to generate API Keys.</param>
        /// <returns>The service container for further chaining.</returns>
        public static IServiceCollection AddDefaultApiKeyGenerator(this IServiceCollection services, ApiKeyGenerationOptions apiKeyGenerationOptions)
        {
            apiKeyGenerationOptions.Validate();
            return services.AddSingleton<IApiKeyFactory, DefaultApiKeyFactory>(sp => new DefaultApiKeyFactory(apiKeyGenerationOptions));
        }

        /// <summary>
        /// Adds the default claims principal factory, see: <seealso cref="DefaultClaimsPrincipalFactory"/>.
        /// </summary>
        /// <param name="services">The container to add the services to.</param>
        /// <returns>The service container for further chaining.</returns>
        public static IServiceCollection AddDefaultClaimsPrincipalFactory(this IServiceCollection services)
        {
            return services.AddSingleton<IClaimsPrincipalFactory, DefaultClaimsPrincipalFactory>();
        }
    }
}
