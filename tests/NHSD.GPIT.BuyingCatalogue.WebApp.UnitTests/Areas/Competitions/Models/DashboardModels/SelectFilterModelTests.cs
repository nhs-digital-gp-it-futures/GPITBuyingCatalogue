using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.DashboardModels;

public static class SelectFilterModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_PropertiesSetAsExpected(
        string organisationName,
        List<Filter> filters)
    {
        var model = new SelectFilterModel(organisationName, filters);

        model.OrganisationName.Should().Be(organisationName);
        model.Filters.Should().BeEquivalentTo(filters.Select(x => new SelectOption<string>(x.Name, x.Id.ToString())));
    }
}
