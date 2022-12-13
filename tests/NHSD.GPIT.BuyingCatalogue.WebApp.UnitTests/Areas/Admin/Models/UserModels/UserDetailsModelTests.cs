using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.UserModels
{
    public static class UserDetailsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithUserConstruction_PropertiesSetAsExpected(
            AspNetUser user)
        {
            var actual = new UserDetailsModel(user);

            actual.SelectedOrganisationId.Should().Be(user.PrimaryOrganisationId);
            actual.UserId.Should().Be(user.Id);
            actual.Title.Should().Be("Edit user");
            actual.FirstName.Should().Be(user.FirstName);
        }
    }
}
