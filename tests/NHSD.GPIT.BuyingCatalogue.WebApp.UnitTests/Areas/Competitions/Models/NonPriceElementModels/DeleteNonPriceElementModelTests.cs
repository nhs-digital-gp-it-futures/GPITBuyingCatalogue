using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels;

public static class DeleteNonPriceElementModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        NonPriceElement nonPriceElement,
        Competition competition,
        IEnumerable<Integration> integrations)
    {
        var model = new DeleteNonPriceElementModel(nonPriceElement, competition, integrations);

        model.NonPriceElement.Should().Be(nonPriceElement);
    }
}
