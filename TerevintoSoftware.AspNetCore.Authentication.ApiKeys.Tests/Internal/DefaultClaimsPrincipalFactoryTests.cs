using NUnit.Framework;
using System.Security.Claims;
using System.Threading.Tasks;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Internal;

namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Tests.Internal
{
    [TestFixture]
    public class DefaultClaimsPrincipalFactoryTests
    {
        [Test]
        public async Task CreateClaimsPrincipal_ShouldCreateAPrincipalWithTheKeyGiven()
        {
            var key = "CT-123456";
            var factory = new DefaultClaimsPrincipalFactory();

            var principal = await factory.CreateClaimsPrincipal(key);

            Assert.NotNull(principal);
            Assert.IsTrue(principal.HasClaim(ClaimTypes.Name, key));
        }

        [Test]
        public async Task CreateClaimsPrincipal_ShouldUseTheApiKeyAuthenticationName()
        {
            var key = "CT-123456";
            var factory = new DefaultClaimsPrincipalFactory();

            var principal = await factory.CreateClaimsPrincipal(key);

            Assert.NotNull(principal);
            Assert.IsTrue(principal.Identity!.AuthenticationType == ApiKeyAuthenticationOptions.DefaultScheme);
        }
    }
}
