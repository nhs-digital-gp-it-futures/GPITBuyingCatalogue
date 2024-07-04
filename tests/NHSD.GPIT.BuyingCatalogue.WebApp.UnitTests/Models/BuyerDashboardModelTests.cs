using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models;

public static class BuyerDashboardModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(
        Organisation organisation,
        List<Order> orders,
        List<Competition> competitions,
        List<Filter> filters)
    {
        var model = new BuyerDashboardModel(organisation, orders, competitions, filters);

        model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
        model.OrganisationName.Should().Be(organisation.Name);
        model.Orders.Should().BeEquivalentTo(orders);
        model.Competitions.Should()
            .BeEquivalentTo(
                competitions.Select(
                    x => new CompetitionDashboardItem(x)));
        model.Shortlists.Should().BeEquivalentTo(filters);
    }
}
