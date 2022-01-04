using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.OrganisationModelsTests
{
    public static class EditOrganisationModelTests
    {
        [Fact]
        public static void Constructor_NullOrganisation_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new EditOrganisationModel(null));

            actual.ParamName.Should().Be("organisation");
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            Organisation organisation)
        {
            var actual = new EditOrganisationModel(organisation);

            actual.CatalogueAgreementSigned.Should().Be(organisation.CatalogueAgreementSigned);
            actual.Organisation.Should().Be(organisation);
            actual.OrganisationAddress.Should().Be(organisation.Address);
        }
    }
}
