using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.OdsOrganisations;

public static class OdsOrganisationTests
{
    [Theory]
    [CommonAutoData]
    public static void UpdateFrom_NullSource(
        string name,
        bool isActive,
        string addressLine1,
        string addressLine2,
        string addressLine3,
        string town,
        string county,
        string postcode,
        string country)
    {
        var odsOrganisation = new OdsOrganisation
        {
            Name = name,
            IsActive = isActive,
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            AddressLine3 = addressLine3,
            Town = town,
            County = county,
            Postcode = postcode,
            Country = country,
        };

        odsOrganisation.UpdateFrom(null);

        odsOrganisation.Name.Should().Be(name);
        odsOrganisation.IsActive.Should().Be(isActive);
        odsOrganisation.AddressLine1.Should().Be(addressLine1);
        odsOrganisation.AddressLine2.Should().Be(addressLine2);
        odsOrganisation.AddressLine3.Should().Be(addressLine3);
        odsOrganisation.Town.Should().Be(town);
        odsOrganisation.County.Should().Be(county);
        odsOrganisation.Postcode.Should().Be(postcode);
        odsOrganisation.Country.Should().Be(country);
    }

    [Theory]
    [CommonAutoData]
    public static void UpdateFrom_Expected(
        OdsOrganisation existing,
        OdsOrganisation updated)
    {
        existing.UpdateFrom(updated);

        existing.Should().BeEquivalentTo(updated, opt => opt.Excluding(m => m.Id));
    }
}
