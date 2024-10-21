using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.CatalogueSolutionsModels;

public static class SolutionModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Solution solution)
    {
        var model = new SolutionModel(solution.CatalogueItem);

        model.SolutionId.Should().Be(solution.CatalogueItemId);
        model.SupplierId.Should().Be(solution.CatalogueItem.SupplierId);
        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.SolutionDisplayName.Should().Be(solution.CatalogueItem.Name);
        model.IsPilotSolution.Should().Be(solution.IsPilotSolution);
        model.SelectedCategory.Should().Be(solution.Category);
    }

    [Fact]
    public static void Adding_SetsHeadingAndDescription()
    {
        var model = new SolutionModel();

        model.Heading.Should().Be(SolutionModel.AddHeading);
        model.Description.Should().Be(SolutionModel.AddDescription);
    }

    [Theory]
    [MockAutoData]
    public static void Editing_SetsHeadingAndDescription(
        Solution solution)
    {
        var model = new SolutionModel(solution.CatalogueItem);

        model.Heading.Should().Be(SolutionModel.EditHeading);
        model.Description.Should().Be(SolutionModel.EditDescription);
    }
}
