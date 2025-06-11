using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionStandards;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.CatalogueSolutionStandards;

public static class StandardPartialModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        CatalogueItemId solutionId,
        StandardType standardType,
        List<StandardComplianceModel> standards)
    {
        var model = new StandardPartialModel(solutionId, standardType, standards);

        model.SolutionId.Should().Be(solutionId);
        model.StandardType.Should().Be(standardType);
        model.Standards.Should().BeEquivalentTo(standards);
    }
}
