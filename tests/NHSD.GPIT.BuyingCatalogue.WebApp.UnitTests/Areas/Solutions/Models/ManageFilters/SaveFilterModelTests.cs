using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
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
            Dictionary<string, IOrderedEnumerable<Epic>> capabiltiesAndEpics,
            EntityFramework.Catalogue.Models.Framework framework,
            List<ApplicationType> applicationTypes,
            List<HostingType> hostingTypes,
            List<InteropIm1IntegrationType> iM1IntegrationsTypes,
            List<InteropGpConnectIntegrationType> gPConnectIntegrationsTypes,
            List<InteropIntegrationType> interoperabilityIntegrationTypes,
            int organisationId)
        {
            var model = new SaveFilterModel(capabiltiesAndEpics, framework, applicationTypes, hostingTypes, iM1IntegrationsTypes, gPConnectIntegrationsTypes, interoperabilityIntegrationTypes, organisationId);

            model.FrameworkId.Should().Be(framework.Id);
            model.FrameworkName.Should().Be(framework.ShortName);

            model.ApplicationTypes.Should().BeEquivalentTo(applicationTypes);
            model.HostingTypes.Should().BeEquivalentTo(hostingTypes);
            model.IM1IntegrationsTypes.Should().BeEquivalentTo(iM1IntegrationsTypes);
            model.GPConnectIntegrationsTypes.Should().BeEquivalentTo(gPConnectIntegrationsTypes);
            model.InteroperabilityIntegrationTypes.Should().BeEquivalentTo(interoperabilityIntegrationTypes);
            model.OrganisationId.Should().Be(organisationId);
            model.GroupedCapabilities.Should().BeEquivalentTo(capabiltiesAndEpics);
        }
    }
}
