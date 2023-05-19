using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models.ManageFilters
{
    public static class ManageFilterModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_PropertiesAreSetCorrectly(
            List<Filter> filters,
            string orgName)
        {
            var model = new ManageFiltersModel(filters, orgName);

            model.Filters.Count.Should().Be(filters.Count);
            model.Filters.Should().BeEquivalentTo(filters);
            model.OrganisationName.Should().Be(orgName);
        }
    }
}
