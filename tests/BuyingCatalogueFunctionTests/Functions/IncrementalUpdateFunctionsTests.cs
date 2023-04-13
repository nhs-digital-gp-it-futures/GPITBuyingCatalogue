using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.Functions;
using BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Functions
{
    public static class IncrementalUpdateFunctionsTests
    {
        [Fact]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(IncrementalUpdateFunctions).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task IncrementalUpdateHttpTrigger_UpdateServiceThrowsError_ExpectedResult(
            [Frozen] Mock<HttpRequestData> httpRequestData,
            [Frozen] Mock<HttpResponseData> httpResponseData,
            [Frozen] Mock<IIncrementalUpdateService> updateService,
            IncrementalUpdateFunctions functions)
        {
            updateService
                .Setup(x => x.UpdateOrganisationData())
                .Throws<ArgumentNullException>();

            httpResponseData.SetupGet(x => x.StatusCode)
                .Returns(HttpStatusCode.InternalServerError);

            httpRequestData.Setup(x => x.CreateResponse())
                .Returns(httpResponseData.Object);

            var result = await functions.IncrementalUpdateHttpTrigger(httpRequestData.Object);

            updateService.VerifyAll();

            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Theory]
        [CommonAutoData]
        public static async Task IncrementalUpdateHttpTrigger_EverythingOk_ExpectedResult(
            [Frozen] Mock<HttpRequestData> httpRequestData,
            [Frozen] Mock<HttpResponseData> httpResponseData,
            [Frozen] Mock<IIncrementalUpdateService> updateService,
            IncrementalUpdateFunctions functions)
        {
            updateService
                .Setup(x => x.UpdateOrganisationData())
                .Verifiable();

            httpResponseData.SetupGet(x => x.StatusCode)
                .Returns(HttpStatusCode.OK);

            httpRequestData.Setup(x => x.CreateResponse())
                .Returns(httpResponseData.Object);

            var result = await functions.IncrementalUpdateHttpTrigger(httpRequestData.Object);

            updateService.VerifyAll();

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [CommonAutoData]
        public static async Task IncrementalUpdateTimerTrigger_ExpectedResult(
            [Frozen] Mock<IIncrementalUpdateService> updateService,
            IncrementalUpdateFunctions functions)
        {
            updateService
                .Setup(x => x.UpdateOrganisationData())
                .Verifiable();

            await functions.IncrementalUpdateTimerTrigger(null);

            updateService.VerifyAll();
        }
    }
}
