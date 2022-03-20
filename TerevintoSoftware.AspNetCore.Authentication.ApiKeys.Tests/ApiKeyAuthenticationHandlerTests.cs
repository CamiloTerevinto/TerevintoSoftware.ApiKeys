using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Abstractions;
using TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Internal;

namespace TerevintoSoftware.AspNetCore.Authentication.ApiKeys.Tests
{
    [TestFixture]
    public class ApiKeyAuthenticationHandlerTests
    {
        private readonly ApiKeyAuthenticationOptions _apiKeyAuthenticationOptions = new();
        private readonly Mock<ILogger<ApiKeyAuthenticationHandler>> _logger = new();
        private readonly Mock<IApiKeysCacheService> _apiKeysCacheServiceMock = new Mock<IApiKeysCacheService>();

        private ApiKeyAuthenticationHandler _handler = default!;

        [SetUp]
        public void SetUp()
        {
            var optionsMonitorMock = new Mock<IOptionsMonitor<ApiKeyAuthenticationOptions>>();

            optionsMonitorMock.Setup(x => x.Get(It.IsAny<string>())).Returns(_apiKeyAuthenticationOptions);
            optionsMonitorMock.Setup(x => x.CurrentValue).Returns(_apiKeyAuthenticationOptions);

            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_logger.Object);

            var urlEncoderMock = new Mock<UrlEncoder>();
            var systemClockMock = new Mock<ISystemClock>();
            var claimsPrincipalFactoryMock = new DefaultClaimsPrincipalFactory();

            _handler = new ApiKeyAuthenticationHandler(optionsMonitorMock.Object, loggerFactoryMock.Object,
                urlEncoderMock.Object, systemClockMock.Object, _apiKeysCacheServiceMock.Object, claimsPrincipalFactoryMock);
        }

        [Test]
        public async Task HandleAuthenticateAsync_WithoutAuthorizationHeader_ShouldFail()
        {
            var context = new DefaultHttpContext();
            await _handler.InitializeAsync(
                new AuthenticationScheme(ApiKeyAuthenticationOptions.DefaultScheme, null, typeof(ApiKeyAuthenticationHandler)), context);

            var result = await _handler.AuthenticateAsync();

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(_apiKeyAuthenticationOptions.FailureMessage, result.Failure!.Message);
        }

        [Test]
        public async Task HandleAuthenticateAsync_WithInvalidApiKey_ShouldFail()
        {
            var apiKey = "CT-123";
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(ApiKeyAuthenticationOptions.HeaderName, apiKey);
            await _handler.InitializeAsync(
                new AuthenticationScheme(ApiKeyAuthenticationOptions.DefaultScheme, null, typeof(ApiKeyAuthenticationHandler)), context);

            _apiKeysCacheServiceMock.Setup(x => x.GetOwnerIdFromApiKey(apiKey)).Returns(null);

            var result = await _handler.AuthenticateAsync();

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(_apiKeyAuthenticationOptions.FailureMessage, result.Failure!.Message);
        }

        [Test]
        public async Task HandleAuthenticateAsync_WithValidApiKey_ShouldCreateAValidTicket()
        {
            var apiKey = "CT-123";
            var ownerId = "owner";
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(ApiKeyAuthenticationOptions.HeaderName, apiKey);
            await _handler.InitializeAsync(
                new AuthenticationScheme(ApiKeyAuthenticationOptions.DefaultScheme, null, typeof(ApiKeyAuthenticationHandler)), context);

            _apiKeysCacheServiceMock.Setup(x => x.GetOwnerIdFromApiKey(apiKey)).ReturnsAsync(ownerId);

            var result = await _handler.AuthenticateAsync();

            Assert.IsTrue(result.Succeeded);
            Assert.NotNull(result.Ticket);
            Assert.AreEqual(ApiKeyAuthenticationOptions.DefaultScheme, result.Ticket!.AuthenticationScheme);
            Assert.AreEqual(ownerId, result.Principal!.Identity!.Name);
        }
    }
}
