using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.OrganisationModelsTests
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
        }
    }
}
