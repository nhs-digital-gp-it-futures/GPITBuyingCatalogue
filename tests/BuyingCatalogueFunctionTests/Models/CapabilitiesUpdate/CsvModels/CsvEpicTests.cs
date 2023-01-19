using System;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.Models.CapabilitiesUpdate.CsvModels;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Models.CapabilitiesUpdate.CsvModels;

public class CsvEpicTests
{
    [Theory]
    [InlineAutoData("MUST", CompliancyLevel.Must)]
    [InlineAutoData("MAY", CompliancyLevel.May)]
    [InlineAutoData("SHOULD", CompliancyLevel.Should)]
    public static void GetCompliancyLevel_Valid_ReturnsExpected(
        string level,
        CompliancyLevel expectedLevel,
        CsvEpic epic)
    {
        epic.Level = level;

        var result = epic.GetCompliancyLevel();

        result.Should().Be(expectedLevel);
    }

    [Theory]
    [CommonAutoData]
    public static void GetCompliancyLevel_InvalidType_ThrowsException(
        string level,
        CsvEpic epic)
    {
        epic.Level = level;

        FluentActions.Invoking(epic.GetCompliancyLevel).Should().Throw<InvalidOperationException>();
    }
}
