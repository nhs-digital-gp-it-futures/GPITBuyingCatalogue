using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.SelectSolutionModels;

public static class SolutionJustificationModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsProperties(
        string justification,
        CatalogueItem catalogueItem,
        Supplier supplier)
    {
        catalogueItem.Supplier = supplier;

        var model = new SolutionJustificationModel(catalogueItem, justification);

        model.SolutionId.Should().Be(catalogueItem.Id);
        model.SolutionName.Should().Be(catalogueItem.Name);
        model.SupplierName.Should().Be(supplier.Name);
        model.Justification.Should().Be(justification);
    }
}
