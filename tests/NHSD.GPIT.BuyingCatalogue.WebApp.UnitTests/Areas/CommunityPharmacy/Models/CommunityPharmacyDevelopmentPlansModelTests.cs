using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.CommunityPharmacy.Models;

public static class CommunityPharmacyDevelopmentPlansModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_MapsRoadmapAndWorkOffPlans(
        Solution solution,
        List<WorkOffPlan> workOffPlans,
        CatalogueItemContentStatus contentStatus)
    {
        var model = new CommunityPharmacyDevelopmentPlansModel(solution.CatalogueItem, workOffPlans, contentStatus);

        model.RoadmapLink.Should().Be(solution.RoadMap);
        model.WorkOffPlans.Should().BeSameAs(workOffPlans);
        model.SelectedSection.Should().Be("Development plans");
    }
}
