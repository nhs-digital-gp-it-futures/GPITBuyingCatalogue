using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models.ManageFilters
{
    public static class SaveFilterModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_PropertiesAreSetCorrectly(
            List<Capability> capabilities,
            List<Epic> epics,
            EntityFramework.Catalogue.Models.Framework framework,
            List<ClientApplicationType> clientApplicationTypes,
            List<HostingType> hostingTypes,
            int organisationId)
        {
            var model = new SaveFilterModel(capabilities, epics, framework, clientApplicationTypes, hostingTypes, organisationId);

            model.CapabilityIds.Count.Should().Be(capabilities.Count);
            model.EpicIds.Count.Should().Be(epics.Count);

            model.FrameworkId.Should().Be(framework.Id);
            model.FrameworkName.Should().Be(framework.ShortName);

            model.ClientApplicationTypes.Should().BeEquivalentTo(clientApplicationTypes);
            model.HostingTypes.Should().BeEquivalentTo(hostingTypes);
            model.OrganisationId.Should().Be(organisationId);
        }

        [Theory]
        [CommonAutoData]
        public static void SetGroupedCapabilities_PropertiesAreSetCorrectly(
            List<Capability> capabilities,
            List<Epic> epics,
            SaveFilterModel model)
        {
            for (var i = 0; i < capabilities.Count; i++)
            {
                epics[i].Capabilities.Add(capabilities[i]);
            }

            model.SetGroupedCapabilities(capabilities, epics);

            model.GroupedCapabilities.Count.Should().Be(capabilities.Count);

            foreach (var capability in capabilities)
            {
                var expected = epics.Where(x => x.Capabilities.Any(y => y.Id == capability.Id));

                model.GroupedCapabilities[capability.Name].Should().NotBeNull();

                model.GroupedCapabilities[capability.Name].Should().BeEquivalentTo(expected);
            }
        }
    }
}
