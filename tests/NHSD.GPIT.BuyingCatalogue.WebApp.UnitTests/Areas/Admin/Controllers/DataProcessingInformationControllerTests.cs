using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers;

public static class DataProcessingInformationControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(DataProcessingInformationController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Solution solution,
        [Frozen] IDataProcessingInformationService dataProcessingInformationService,
        DataProcessingInformationController controller)
    {
        dataProcessingInformationService.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId)
            .Returns(solution);

        var expectedModel = new DataProcessingInformationModel(solution);

        var result = (await controller.Index(solution.CatalogueItemId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }
}
