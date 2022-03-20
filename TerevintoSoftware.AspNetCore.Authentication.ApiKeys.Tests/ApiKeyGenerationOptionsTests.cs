using NUnit.Framework;
using System;

namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Tests
{
    [TestFixture]
    public class ApiKeyGenerationOptionsTests
    {
        [Test]
        public void Validate_WithValidConfiguration_ShouldSucceed()
        {
            var options = new ApiKeyGenerationOptions
            {
                KeyPrefix = "CT-",
                GenerateUrlSafeKeys = true,
                LengthOfKey = 32
            };

            options.Validate();
        }

        [Test]
        public void Validate_WithNullKeyPrefix_ShouldAssignEmptyString()
        {
            var options = new ApiKeyGenerationOptions
            {
                KeyPrefix = null,
                GenerateUrlSafeKeys = true,
                LengthOfKey = 32
            };

            options.Validate();

            Assert.AreEqual("", options.KeyPrefix);
        }

        [Test]
        public void Validate_WithInvalidLengthOfKey_ShouldThrow()
        {
            var options = new ApiKeyGenerationOptions
            {
                KeyPrefix = "CT-",
                GenerateUrlSafeKeys = true,
                LengthOfKey = 0
            };

            Assert.Throws<InvalidOperationException>(() => options.Validate(),
                "A positive, non-zero length of key is required.");
        }

        [Test]
        public void Validate_WithLengthOfKeySmallerThanKeyPrefix_ShouldThrow()
        {
            var options = new ApiKeyGenerationOptions
            {
                KeyPrefix = "CT-",
                GenerateUrlSafeKeys = true,
                LengthOfKey = 1
            };

            Assert.Throws<InvalidOperationException>(() => options.Validate(),
                "The length of the key must be bigger than the key prefix.");
        }
    }
}
