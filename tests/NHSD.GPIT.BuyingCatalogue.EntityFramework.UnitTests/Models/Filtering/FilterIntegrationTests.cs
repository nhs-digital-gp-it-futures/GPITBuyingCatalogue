using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Filtering;

public static class FilterIntegrationTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        SupportedIntegrations integrationId)
    {
        var model = new FilterIntegration(integrationId);

        model.FilterId.Should().Be(default);
        model.IntegrationId.Should().Be(integrationId);
        model.IntegrationTypes.Should().BeEmpty();
        model.Integration.Should().BeNull();
        model.Filter.Should().BeNull();
    }
}
