using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SharedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.SelectSolutionModels;

public static class SolutionModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsProperties(
        CatalogueItem catalogueItem,
        Supplier supplier,
        List<string> requiredServices,
        bool selected)
    {
        catalogueItem.Supplier = supplier;

        var model = new SolutionModel(catalogueItem, requiredServices, selected);

        model.SolutionId.Should().Be(catalogueItem.Id);
        model.SolutionName.Should().Be(catalogueItem.Name);
        model.SupplierName.Should().Be(supplier.Name);
        model.RequiredServices.Should().BeEquivalentTo(requiredServices);
        model.Selected.Should().Be(selected);
    }

    [Theory]
    [CommonAutoData]
    public static void GetAdditionalServicesList_ReturnsCommaSeparatedList(
        List<string> requiredServices,
        SolutionModel model)
    {
        var expected = string.Join(", ", requiredServices);

        model.RequiredServices = requiredServices;

        model.GetAdditionalServicesList().Should().Be(expected);
    }
}
