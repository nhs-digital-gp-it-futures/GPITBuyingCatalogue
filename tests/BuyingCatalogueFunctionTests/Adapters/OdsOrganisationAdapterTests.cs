using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using BuyingCatalogueFunction.Adapters;
using BuyingCatalogueFunction.Models.Ods;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Adapters
{
    public static class OdsOrganisationAdapterTests
    {
        [Fact]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OdsOrganisationAdapter).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void Process_InputIsNull_ThrowsException(
            OdsOrganisationAdapter adapter)
        {
            FluentActions
                .Invoking(() => adapter.Process(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void Process_InputIsNotNull_ReturnsExpectedResult(
            Org input,
            OdsOrganisationAdapter adapter)
        {
            var result = adapter.Process(input);

            result.Should().NotBeNull();

            result.Id.Should().Be(input.OrgId.extension);
            result.Name.Should().Be(input.Name);
            result.IsActive.Should().Be(input.Status == Org.Active);
            result.AddressLine1.Should().Be(input.GeoLoc.Location.AddrLn1);
            result.AddressLine2.Should().Be(input.GeoLoc.Location.AddrLn2);
            result.AddressLine3.Should().Be(input.GeoLoc.Location.AddrLn3);
            result.Town.Should().Be(input.GeoLoc.Location.Town);
            result.County.Should().Be(input.GeoLoc.Location.County);
            result.Postcode.Should().Be(input.GeoLoc.Location.PostCode);
            result.Country.Should().Be(input.GeoLoc.Location.Country);
        }
    }
}
