using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared.Partials;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.Shared;

public static class InteroperabilityPartialModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Dictionary<SupportedIntegrations, string> integrations,
        ICollection<IntegrationType> integrationTypes)
    {
        var model = new InteroperabilityPartialModel(integrations, integrationTypes);

        model.AvailableIntegrations.Should().BeEquivalentTo(integrations);
        model.IntegrationTypes.Should().BeEquivalentTo(integrationTypes.ToList());
    }
}
