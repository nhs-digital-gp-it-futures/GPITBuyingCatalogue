using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Security;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Security;

public static class GoogleRecaptchaVerificationServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(GoogleRecaptchaVerificationService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockInlineAutoData(null)]
    [MockInlineAutoData("")]
    public static async Task Validate_NullOrEmptyRecaptchaResponse_ReturnsFalse(
        string recaptchaResponse,
        GoogleRecaptchaVerificationService service)
    {
        var result = await service.Validate(recaptchaResponse);

        result.Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static Task Validate_UnsuccessfulHttpResponse_ThrowsException(
        string recaptchaResponse,
        RecaptchaSettings settings,
        [Frozen] HttpMessageHandler handler,
        [Frozen] ILogger<GoogleRecaptchaVerificationService> logger)
    {
        var flags = BindingFlags.NonPublic | BindingFlags.Instance;
        handler.GetType().GetMethod("SendAsync", flags)!
            .Invoke(handler, new object[] { Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>() })
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));

        var httpClient = new HttpClient(handler) { BaseAddress = new("https://www.google.com") };
        var service = new GoogleRecaptchaVerificationService(httpClient, Options.Create(settings), logger);

        return FluentActions.Awaiting(() => service.Validate(recaptchaResponse))
            .Should()
            .ThrowAsync<HttpRequestException>();
    }

    [Theory]
    [MockInlineAutoData(true)]
    [MockInlineAutoData(false)]
    public static async Task Validate_SuccessfulHttpResponse_ReturnsExpected(
        string recaptchaResponse,
        bool expected,
        RecaptchaSettings settings,
        [Frozen] HttpMessageHandler handler,
        [Frozen] ILogger<GoogleRecaptchaVerificationService> logger)
    {
        var response = new GoogleRecaptchaVerificationService.GoogleRecaptchaResponse { Success = expected, };

        var flags = BindingFlags.NonPublic | BindingFlags.Instance;
        handler.GetType().GetMethod("SendAsync", flags)!
            .Invoke(handler, new object[] { Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>() })
            .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(response)),
            }));

        var httpClient = new HttpClient(handler) { BaseAddress = new("https://www.google.com") };
        var service = new GoogleRecaptchaVerificationService(httpClient, Options.Create(settings), logger);

        var result = await service.Validate(recaptchaResponse);

        result.Should().Be(expected);
    }
}
