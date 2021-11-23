using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class AddUserConfirmationModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            string userName,
            int organisationId)
        {
            var actual = new AddUserConfirmationModel(userName, organisationId);

            actual.BackLinkText.Should().Be("Back to dashboard");
            actual.Name.Should().Be(userName);
        }
    }
}
