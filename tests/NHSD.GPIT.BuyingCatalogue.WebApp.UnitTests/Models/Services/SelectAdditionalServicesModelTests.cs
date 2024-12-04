using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Services;

public static class SelectAdditionalServicesModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        List<CatalogueItem> excludedServices,
        List<CatalogueItem> allServices)
    {
        var model = new SelectAdditionalServicesModel(excludedServices, allServices);

        model.ExistingServices.Should().BeEquivalentTo(excludedServices.Select(x => x.Name));
    }
}
