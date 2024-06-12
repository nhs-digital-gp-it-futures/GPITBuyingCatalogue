using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Competitions;

public static class FeaturesCriteriaTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        string requirements,
        CompliancyLevel compliance)
    {
        var model = new FeaturesCriteria(requirements, compliance);

        model.Requirements.Should().Be(requirements);
        model.Compliance.Should().Be(compliance);
    }
}
