using System;
using System.Threading.Tasks;
using System.Web.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.Functions;
using BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
            [Frozen] Mock<IIncrementalUpdateService> updateService,
            IncrementalUpdateFunctions functions)
        {
            updateService
                .Setup(x => x.UpdateOrganisationData())
                .Throws<ArgumentNullException>();

            var result = await functions.IncrementalUpdateHttpTrigger(null);

            updateService.VerifyAll();

            result.Should().BeAssignableTo<InternalServerErrorResult>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task IncrementalUpdateHttpTrigger_EverythingOk_ExpectedResult(
            [Frozen] Mock<IIncrementalUpdateService> updateService,
            IncrementalUpdateFunctions functions)
        {
            updateService
                .Setup(x => x.UpdateOrganisationData())
                .Verifiable();

            var result = await functions.IncrementalUpdateHttpTrigger(null);

            updateService.VerifyAll();

            result.Should().BeAssignableTo<OkResult>();
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
