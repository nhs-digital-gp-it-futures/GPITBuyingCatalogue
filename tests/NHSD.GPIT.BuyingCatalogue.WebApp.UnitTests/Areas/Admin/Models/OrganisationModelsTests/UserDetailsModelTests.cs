using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.OrganisationModelsTests
{
    public static class UserDetailsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            Organisation organisation,
            AspNetUser user)
        {
            var model = new UserDetailsModel(organisation, user);

            model.Organisation.Should().Be(organisation);
            model.User.Should().Be(user);
        }

        [Theory]
        [CommonAutoData]
        public static void WithUserDisabled_TogglesSetCorrectly(
            Organisation organisation,
            AspNetUser user)
        {
            user.Disabled = true;

            var model = new UserDetailsModel(organisation, user);

            model.Toggle.Should().Be("UserEnabled");
            model.ToggleText.Should().Be("Re-enable account");
        }

        [Theory]
        [CommonAutoData]
        public static void WithUserEnabled_TogglesSetCorrectly(
            Organisation organisation,
            AspNetUser user)
        {
            user.Disabled = false;

            var model = new UserDetailsModel(organisation, user);

            model.Toggle.Should().Be("UserDisabled");
            model.ToggleText.Should().Be("Disable account");
        }
    }
}
