using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.SupplierDefinedEpics
{
    public static class SupplierDefinedEpicsDashboardModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Constructor_PropertiesAreSetCorrectly(
            IList<Epic> epics,
            string searchTerm)
        {
            var models = epics.Select(x => new SupplierDefinedEpicModel(x));

            var systemUnderTest = new SupplierDefinedEpicsDashboardModel(epics, searchTerm);

            systemUnderTest.SupplierDefinedEpics.Should().BeEquivalentTo(models);
            systemUnderTest.SearchTerm.Should().Be(searchTerm);
        }
    }
}
