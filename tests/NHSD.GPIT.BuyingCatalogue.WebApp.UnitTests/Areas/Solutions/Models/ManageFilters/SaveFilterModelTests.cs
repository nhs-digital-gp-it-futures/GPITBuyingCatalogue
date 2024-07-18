using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models.ManageFilters
{
    public static class SaveFilterModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Constructor_PropertiesAreSetCorrectly(
            Dictionary<string, IOrderedEnumerable<Epic>> capabilitiesAndEpics,
            EntityFramework.Catalogue.Models.Framework framework,
            List<ApplicationType> applicationTypes,
            List<HostingType> hostingTypes,
            Dictionary<string, IOrderedEnumerable<string>> integrations,
            int organisationId)
        {
            var model = new SaveFilterModel(capabilitiesAndEpics, framework, applicationTypes, hostingTypes, integrations, organisationId);

            model.FrameworkId.Should().Be(framework.Id);
            model.FrameworkName.Should().Be(framework.ShortName);

            model.ApplicationTypes.Should().BeEquivalentTo(applicationTypes);
            model.HostingTypes.Should().BeEquivalentTo(hostingTypes);
            model.Integrations.Should().BeEquivalentTo(integrations);
            model.OrganisationId.Should().Be(organisationId);
            model.GroupedCapabilities.Should().BeEquivalentTo(capabilitiesAndEpics);
        }
    }
}
