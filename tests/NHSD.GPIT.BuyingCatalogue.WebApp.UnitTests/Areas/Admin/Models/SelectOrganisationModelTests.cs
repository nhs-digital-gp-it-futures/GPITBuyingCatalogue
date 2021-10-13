using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class SelectOrganisationModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            OdsOrganisation organisation)
        {
            var model = new SelectOrganisationModel(organisation);

            model.BackLinkText.Should().Be("Go back to previous page");
            model.BackLink.Should().Be($"/admin/organisations/find");
            model.OdsOrganisation.Should().Be(organisation);
        }
    }
}
