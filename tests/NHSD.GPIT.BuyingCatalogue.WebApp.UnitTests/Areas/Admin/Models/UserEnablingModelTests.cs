using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class UserEnablingModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            Organisation organisation,
            AspNetUser user)
        {
            var model = new UserEnablingModel(organisation, user);

            model.Organisation.Should().Be(organisation);
            model.User.Should().Be(user);
        }
    }
}
