using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.CapabilityModels;

public static class CapabilityCategoryModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(
        Solution solution,
        CapabilityCategory capabilityCategory)
    {
        var model = new CapabilityCategoryModel(solution.CatalogueItem, capabilityCategory);

        model.Name.Should().Be(capabilityCategory.Name);
        model.Capabilities.Should().HaveCount(capabilityCategory.Capabilities.Count);
    }
}
