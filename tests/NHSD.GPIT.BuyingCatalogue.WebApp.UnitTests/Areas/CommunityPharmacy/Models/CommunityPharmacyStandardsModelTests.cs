using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.CommunityPharmacy.Models;

public static class CommunityPharmacyStandardsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_WithStandards_OnlyIncludesFullyMetAndInProgress(
        Standard standard,
        Solution solution,
        List<string> standardsWithWorkOffPlans,
        CatalogueItemContentStatus contentStatus)
    {
        var standards = Enum.GetValues<StandardCompliance>()
            .Select(x => new StandardComplianceModel(standard, x));

        var model = new CommunityPharmacyStandardsModel(
            solution.CatalogueItem, standards, standardsWithWorkOffPlans, contentStatus);

        model.Standards.Should().HaveCount(2);
        model.Standards.Should().Contain(x => x.Compliance == StandardCompliance.FullyMet);
        model.Standards.Should().Contain(x => x.Compliance == StandardCompliance.InProgress);
        model.StandardsWithWorkOffPlans.Should().BeSameAs(standardsWithWorkOffPlans);
        model.SelectedSection.Should().Be("Standards");
    }

    [Theory]
    [MockAutoData]
    public static void Construct_NoStandards_ReturnsEmptyList(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        var model = new CommunityPharmacyStandardsModel(
            solution.CatalogueItem, Array.Empty<StandardComplianceModel>(), Array.Empty<string>(), contentStatus);

        model.Standards.Should().BeEmpty();
        model.StandardsWithWorkOffPlans.Should().BeEmpty();
    }
}
