using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class CreateOrganisationModelTests
    {
        [Fact]
        public static void Constructor_NullOrganisation_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new CreateOrganisationModel(null));

            actual.ParamName.Should().Be("organisation");
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            OdsOrganisation organisation)
        {
            var actual = new CreateOrganisationModel(organisation);

            actual.OdsOrganisation.Should().Be(organisation);
            actual.CatalogueAgreementSigned.Should().Be(default);
            actual.BackLinkText.Should().Be("Go back to previous page");
            actual.BackLink.Should().Be($"/admin/organisations/find/select?ods={organisation.OdsCode}");
        }
    }
}
