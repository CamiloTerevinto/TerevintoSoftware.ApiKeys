using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Linq;
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
        private readonly Mock<IApiKeysCacheService> _apiKeysCacheServiceMock = new();
        private Mock<ILogger<ApiKeyAuthenticationHandler>> _logger = default!;

        private ApiKeyAuthenticationHandler _handler = default!;

        [SetUp]
        public void SetUp()
        {
            var optionsMonitorMock = new Mock<IOptionsMonitor<ApiKeyAuthenticationOptions>>();

            optionsMonitorMock.Setup(x => x.Get(It.IsAny<string>())).Returns(_apiKeyAuthenticationOptions);
            optionsMonitorMock.Setup(x => x.CurrentValue).Returns(_apiKeyAuthenticationOptions);

            _logger = new Mock<ILogger<ApiKeyAuthenticationHandler>>();
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_logger.Object);

            var urlEncoderMock = new Mock<UrlEncoder>();
            var systemClockMock = new Mock<ISystemClock>();
            var claimsPrincipalFactoryMock = new DefaultClaimsPrincipalFactory();

            _handler = new ApiKeyAuthenticationHandler(optionsMonitorMock.Object, loggerFactoryMock.Object,
                urlEncoderMock.Object, systemClockMock.Object, _apiKeysCacheServiceMock.Object, claimsPrincipalFactoryMock);
        }

        [Test]
        public async Task HandleAuthenticateAsync_WithoutAuthorizationHeader_ShouldLogMessageAndFail()
        {
            var context = new DefaultHttpContext();
            await _handler.InitializeAsync(
                new AuthenticationScheme(ApiKeyAuthenticationOptions.DefaultScheme, null, typeof(ApiKeyAuthenticationHandler)), context);

            var result = await _handler.AuthenticateAsync();

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(_apiKeyAuthenticationOptions.FailureMessage, result.Failure!.Message);

            _logger.Verify(x => x.Log(
                _apiKeyAuthenticationOptions.NoApiKeyHeaderLog!.Value.Item1,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString() == _apiKeyAuthenticationOptions.NoApiKeyHeaderLog.Value!.Item2.Replace("{HeaderName}", ApiKeyAuthenticationOptions.HeaderName)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }

        [Test]
        public async Task HandleAuthenticateAsync_WithoutAuthorizationHeaderAndWithoutLog_ShouldOnlyFail()
        {
            _apiKeyAuthenticationOptions.NoApiKeyHeaderLog = null;
            var context = new DefaultHttpContext();
            await _handler.InitializeAsync(
                new AuthenticationScheme(ApiKeyAuthenticationOptions.DefaultScheme, null, typeof(ApiKeyAuthenticationHandler)), context);

            var result = await _handler.AuthenticateAsync();

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(_apiKeyAuthenticationOptions.FailureMessage, result.Failure!.Message);
            Assert.IsEmpty(_logger.Invocations.Where(x => x.Method.Name == "Log"));
        }

        [Test]
        public async Task HandleAuthenticateAsync_WithInvalidApiKeyAndLogger_ShouldLogMessageAndFail()
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

            _logger.Verify(x => x.Log(
                _apiKeyAuthenticationOptions.InvalidApiKeyLog!.Value.Item1,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString() == _apiKeyAuthenticationOptions.InvalidApiKeyLog.Value!.Item2.Replace("{ApiKey}", apiKey)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }

        [Test]
        public async Task HandleAuthenticateAsync_WithInvalidApiKeyAndWithoutLog_ShouldOnlyFail()
        {
            _apiKeyAuthenticationOptions.InvalidApiKeyLog = null;
            var apiKey = "CT-123";
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(ApiKeyAuthenticationOptions.HeaderName, apiKey);
            await _handler.InitializeAsync(
                new AuthenticationScheme(ApiKeyAuthenticationOptions.DefaultScheme, null, typeof(ApiKeyAuthenticationHandler)), context);

            _apiKeysCacheServiceMock.Setup(x => x.GetOwnerIdFromApiKey(apiKey)).Returns(null);

            var result = await _handler.AuthenticateAsync();

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(_apiKeyAuthenticationOptions.FailureMessage, result.Failure!.Message);
            Assert.IsEmpty(_logger.Invocations.Where(x => x.Method.Name == "Log"));
        }

        [Test]
        public async Task HandleAuthenticateAsync_WithValidApiKeyAndLogger_ShouldLogMessageCreateAValidTicket()
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

            _logger.Verify(x => x.Log(
                _apiKeyAuthenticationOptions.ValidApiKeyLog!.Value.Item1,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString() == _apiKeyAuthenticationOptions.ValidApiKeyLog.Value!.Item2),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }

        [Test]
        public async Task HandleAuthenticateAsync_WithValidApiKeyAndWithoutLog_ShouldOnlyCreateAValidTicket()
        {
            _apiKeyAuthenticationOptions.ValidApiKeyLog = null;

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

            Assert.IsEmpty(_logger.Invocations.Where(x => x.Method.Name == "Log"));
        }
 
    }
}
