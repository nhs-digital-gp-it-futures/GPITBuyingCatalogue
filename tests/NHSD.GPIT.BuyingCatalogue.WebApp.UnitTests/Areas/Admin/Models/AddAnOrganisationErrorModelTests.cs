using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class AddAnOrganisationErrorModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            string odsCode,
            string error)
        {
            var actual = new AddAnOrganisationErrorModel(odsCode, error);

            actual.OdsCode.Should().Be(odsCode);
            actual.Error.Should().Be(error);
            actual.BackLinkText.Should().Be("Back to find an organisation page");
            actual.BackLink.Should().Be("/admin/organisations/find");
        }
    }
}
