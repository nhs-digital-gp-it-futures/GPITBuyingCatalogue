using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.OrganisationModelsTests
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

        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_IsGpPractice_PropertiesSetAsExpected(
            Organisation organisation,
            List<AspNetUser> users,
            List<Organisation> relatedOrganisations)
        {
            organisation.OrganisationType = OrganisationType.GP;

            var actual = new DetailsModel(organisation, users, relatedOrganisations);

            actual.IsGpPractice.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_NotGpPractice_PropertiesSetAsExpected(
            Organisation organisation,
            List<AspNetUser> users,
            List<Organisation> relatedOrganisations)
        {
            organisation.OrganisationType = OrganisationType.IB;

            var actual = new DetailsModel(organisation, users, relatedOrganisations);

            actual.IsGpPractice.Should().BeFalse();
        }
    }
}
