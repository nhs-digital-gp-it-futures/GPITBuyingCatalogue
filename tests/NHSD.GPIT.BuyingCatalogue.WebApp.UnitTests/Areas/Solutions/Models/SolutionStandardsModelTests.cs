using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models;

public static class SolutionStandardsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_WithStandards_MapsCorrectStandards(
        Standard standard,
        Solution solution,
        List<string> standardsWithWorkOffPlans,
        CatalogueItemContentStatus contentStatus)
    {
        var standardsComplianceModels = Enum.GetValues<StandardCompliance>()
            .Select(x => new StandardComplianceModel(standard, x));

        var model = new SolutionStandardsModel(
            solution.CatalogueItem,
            standardsComplianceModels,
            standardsWithWorkOffPlans,
            contentStatus);

        model.Standards.Should().HaveCount(2);
        model.Standards.Should().Contain(x => x.Compliance == StandardCompliance.FullyMet);
        model.Standards.Should().Contain(x => x.Compliance == StandardCompliance.InProgress);
    }
}
