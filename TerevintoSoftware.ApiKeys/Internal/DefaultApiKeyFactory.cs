using System.Security.Cryptography;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions;

namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Internal
{
    /// <summary>
    /// Default implementation for the <see cref="IApiKeyFactory"/> service. 
    /// Uses <see cref="ApiKeyGenerationOptions"/> to generate keys with the secure <see cref="RandomNumberGenerator"/>.
    /// </summary>
    internal sealed class DefaultApiKeyFactory : IApiKeyFactory
    {
        private readonly ApiKeyGenerationOptions _options;

        public DefaultApiKeyFactory(ApiKeyGenerationOptions apiKeyGenerationOptions)
        {
            _options = apiKeyGenerationOptions;
        }

        public string GenerateApiKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(_options.ByteCountToGenerate);

            string base64String = Convert.ToBase64String(bytes);

            if (_options.GenerateUrlSafeKeys)
            {
                base64String = base64String
                    .Replace("/", "")
                    .Replace("+", "")
                    .Replace("=", "");
            }

            return string.Concat(_options.KeyPrefix, base64String.AsSpan(0, _options.LengthOfKey - _options.KeyPrefix!.Length));
        }
    }
}
