using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionStandards;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers;

public static class CatalogueSolutionStandardsControllerTests
{
    [Fact]
    public static void ClassIsCorrectlyDecorated()
    {
        typeof(CatalogueSolutionStandardsController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
        typeof(CatalogueSolutionStandardsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Solution solution,
        List<StandardComplianceModel> standards,
        [Frozen] ISolutionsService solutionsService,
        [Frozen] ISolutionStandardsService solutionStandardsService,
        CatalogueSolutionStandardsController controller)
    {
        solutionsService.GetSolutionName(solution.CatalogueItemId).Returns(solution.CatalogueItem.Name);
        solutionStandardsService.GetSolutionStandards(solution.CatalogueItemId).Returns(standards);

        var result = (await controller.Index(solution.CatalogueItemId)).As<ViewResult>();

        result.Should().NotBeNull();

        result.Model.Should()
            .BeEquivalentTo(
                new CatalogueSolutionStandardsModel(solution.CatalogueItemId, solution.CatalogueItem.Name, standards),
                opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Edit_ReturnsViewWithModel(
        Solution solution,
        StandardComplianceModel standard,
        [Frozen] ISolutionsService solutionsService,
        [Frozen] ISolutionStandardsService solutionStandardsService,
        CatalogueSolutionStandardsController controller)
    {
        solutionsService.GetSolutionName(solution.CatalogueItemId).Returns(solution.CatalogueItem.Name);
        solutionStandardsService.GetSolutionStandard(solution.CatalogueItemId, standard.Id).Returns(standard);

        var result = (await controller.Edit(solution.CatalogueItemId, standard.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                new CatalogueSolutionStandardModel(solution.CatalogueItemId, solution.CatalogueItem.Name, standard),
                opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Edit_InvalidModel_ReturnsViewWithModel(
        CatalogueItemId solutionId,
        string standardId,
        CatalogueSolutionStandardModel model,
        CatalogueSolutionStandardsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Edit(solutionId, standardId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Edit_ValidModel_Redirects(
        CatalogueItemId solutionId,
        string standardId,
        CatalogueSolutionStandardModel model,
        CatalogueSolutionStandardsController controller)
    {
        var result = (await controller.Edit(solutionId, standardId, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    ["area"] = controller.GetType().AreaName(), [nameof(solutionId)] = solutionId,
                });
    }
}
