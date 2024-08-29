using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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

    [Theory]
    [MockAutoData]
    public static async Task AddOrEditDataProcessingInformation_ReturnsViewWithModel(
        Solution solution,
        [Frozen] IDataProcessingInformationService dataProcessingInformationService,
        DataProcessingInformationController controller)
    {
        dataProcessingInformationService.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId)
            .Returns(solution);

        var expectedModel = new AddEditDataProcessingInformationModel(solution);

        var result = (await controller.AddOrEditDataProcessingInformation(solution.CatalogueItemId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task AddOrEditDataProcessingInformation_InvalidModel_ReturnsView(
        CatalogueItemId catalogueItemId,
        AddEditDataProcessingInformationModel model,
        DataProcessingInformationController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.AddOrEditDataProcessingInformation(catalogueItemId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task AddOrEditDataProcessingInformation_ValidModel_Redirects(
        CatalogueItemId catalogueItemId,
        AddEditDataProcessingInformationModel model,
        DataProcessingInformationController controller)
    {
        var result = (await controller.AddOrEditDataProcessingInformation(catalogueItemId, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }
}
