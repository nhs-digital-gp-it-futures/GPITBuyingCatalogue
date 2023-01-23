using System;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.Models.CapabilitiesUpdate.CsvModels;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Models.CapabilitiesUpdate.CsvModels;

public static class CsvStandardTests
{
    [Theory]
    [InlineAutoData("Capability Specific Standard", StandardType.Capability)]
    [InlineAutoData("Interop Standard", StandardType.Interoperability)]
    [InlineAutoData("Overarching Standard", StandardType.Overarching)]
    [InlineAutoData("Context Specific Standard", StandardType.ContextSpecific)]
    public static void GetStandardType_Valid_ReturnsExpected(
        string type,
        StandardType expectedType,
        CsvStandard standard)
    {
        standard.Type = type;

        var result = standard.GetStandardType();

        result.Should().Be(expectedType);
    }

    [Theory]
    [CommonAutoData]
    public static void GetStandardType_InvalidType_ThrowsException(
        string type,
        CsvStandard standard)
    {
        standard.Type = type;

        FluentActions.Invoking(standard.GetStandardType).Should().Throw<InvalidOperationException>();
    }
}
