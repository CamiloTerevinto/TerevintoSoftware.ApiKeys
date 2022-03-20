using NUnit.Framework;
using System;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Internal;

namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Tests.Internal
{
    [TestFixture]
    public class DefaultApiKeyFactoryTests
    {
        [Test]
        public void GenerateApiKey_ShouldGenerateAKeyWithPrefix()
        {
            var options = new ApiKeyGenerationOptions
            {
                GenerateUrlSafeKeys = true,
                KeyPrefix = "CT-",
                LengthOfKey = 32
            };

            var defaultApiKeyFactory = new DefaultApiKeyFactory(options);

            var apiKey = defaultApiKeyFactory.GenerateApiKey();

            Assert.IsTrue(apiKey.StartsWith(options.KeyPrefix));
        }

        [Test]
        public void GenerateApiKey_ShouldGenerateAKeyWithExpectedLength()
        {
            var options = new ApiKeyGenerationOptions
            {
                GenerateUrlSafeKeys = true,
                KeyPrefix = "CT-",
                LengthOfKey = 32
            };

            var defaultApiKeyFactory = new DefaultApiKeyFactory(options);

            var apiKey = defaultApiKeyFactory.GenerateApiKey();

            Assert.AreEqual(options.LengthOfKey, apiKey.Length);
        }

        [Test]
        public void GenerateApiKey_WithUrlSafeDisabled_ShouldNotReplaceSpecialCharacters()
        {
            var options = new ApiKeyGenerationOptions
            {
                GenerateUrlSafeKeys = false,
                KeyPrefix = "CT-",
                LengthOfKey = 32
            };

            var defaultApiKeyFactory = new DefaultApiKeyFactory(options);

            var apiKey = defaultApiKeyFactory.GenerateApiKey();

            var base64Part = apiKey[options.KeyPrefix.Length..];

            Assert.IsFalse(base64Part.Contains('-'));
            Assert.IsFalse(base64Part.Contains('_'));
        }

        [Test]
        public void GenerateApiKey_WithUrlSafeEnabled_ShouldReplaceSpecialCharacters()
        {
            var options = new ApiKeyGenerationOptions
            {
                GenerateUrlSafeKeys = true,
                KeyPrefix = "CT-",
                LengthOfKey = 32,
            };

            var defaultApiKeyFactory = new DefaultApiKeyFactory(options);

            var apiKey = defaultApiKeyFactory.GenerateApiKey();

            var base64Part = apiKey[options.KeyPrefix.Length..];

            Assert.IsFalse(base64Part.Contains('+'));
            Assert.IsFalse(base64Part.Contains('/'));
        }
    }
}
