using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Security;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Security;

public static class GoogleRecaptchaVerificationServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(GoogleRecaptchaVerificationService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonInlineAutoData(null)]
    [CommonInlineAutoData("")]
    public static async Task Validate_NullOrEmptyRecaptchaResponse_ReturnsFalse(
        string recaptchaResponse,
        GoogleRecaptchaVerificationService service)
    {
        var result = await service.Validate(recaptchaResponse);

        result.Should().BeFalse();
    }

    [Theory]
    [CommonAutoData]
    public static Task Validate_UnsuccessfulHttpResponse_ThrowsException(
        string recaptchaResponse,
        RecaptchaSettings settings,
        [Frozen] Mock<HttpMessageHandler> handler,
        [Frozen] Mock<ILogger<GoogleRecaptchaVerificationService>> logger)
    {
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var httpClient = new HttpClient(handler.Object) { BaseAddress = new("https://www.google.com") };
        var service = new GoogleRecaptchaVerificationService(httpClient, Options.Create(settings), logger.Object);

        return FluentActions.Awaiting(() => service.Validate(recaptchaResponse))
            .Should()
            .ThrowAsync<HttpRequestException>();
    }

    [Theory]
    [CommonInlineAutoData(true)]
    [CommonInlineAutoData(false)]
    public static async Task Validate_SuccessfulHttpResponse_ReturnsExpected(
        string recaptchaResponse,
        bool expected,
        RecaptchaSettings settings,
        [Frozen] Mock<HttpMessageHandler> handler,
        [Frozen] Mock<ILogger<GoogleRecaptchaVerificationService>> logger)
    {
        var response = new GoogleRecaptchaVerificationService.GoogleRecaptchaResponse { Success = expected, };

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(response)),
                });

        var httpClient = new HttpClient(handler.Object) { BaseAddress = new("https://www.google.com") };
        var service = new GoogleRecaptchaVerificationService(httpClient, Options.Create(settings), logger.Object);

        var result = await service.Validate(recaptchaResponse);

        result.Should().Be(expected);
    }
}
