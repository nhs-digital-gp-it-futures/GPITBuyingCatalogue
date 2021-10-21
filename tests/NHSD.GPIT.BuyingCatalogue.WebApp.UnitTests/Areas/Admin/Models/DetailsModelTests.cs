using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class DetailsModelTests
    {
        [Fact]
        public static void Constructor_NullOrganisation_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new DetailsModel(null, null, null));

            actual.ParamName.Should().Be("organisation");
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_AgreementNotSigned_PropertiesSetAsExpected(
            Organisation organisation,
            List<AspNetUser> users,
            List<Organisation> relatedOrganisations)
        {
            organisation.CatalogueAgreementSigned = false;

            var actual = new DetailsModel(organisation, users, relatedOrganisations);

            actual.Organisation.Should().Be(organisation);
            actual.CatalogueAgreementText.Should().Be("Organisation End User Agreement has not been signed");
            actual.Users.Should().BeEquivalentTo(users);
            actual.RelatedOrganisations.Should().BeEquivalentTo(relatedOrganisations);
            actual.BackLinkText.Should().Be("Go back to previous page");
            actual.BackLink.Should().Be("/admin/buyer-organisations");
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_AgreementSigned_PropertiesSetAsExpected(
            Organisation organisation,
            List<AspNetUser> users,
            List<Organisation> relatedOrganisations)
        {
            organisation.CatalogueAgreementSigned = true;

            var actual = new DetailsModel(organisation, users, relatedOrganisations);

            actual.CatalogueAgreementText.Should().Be("Organisation End User Agreement has been signed");
        }
    }
}
