using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions;

public static class StandardComplianceModelTests
{
    [Theory]
    [MockInlineAutoData(true, StandardCompliance.InProgress)]
    [MockInlineAutoData(false, StandardCompliance.FullyMet)]
    public static void Construct_SetsPropertiesAsExpected(
        bool isInProgress,
        StandardCompliance expectedCompliance,
        Standard standard)
    {
        var model = new StandardComplianceModel(standard, isInProgress);

        model.Id.Should().Be(standard.Id);
        model.Name.Should().Be(standard.Name);
        model.Description.Should().Be(standard.Description);
        model.Url.Should().Be(standard.Url);
        model.Type.Should().Be(standard.StandardType);
        model.Compliance.Should().Be(expectedCompliance);
    }
}
