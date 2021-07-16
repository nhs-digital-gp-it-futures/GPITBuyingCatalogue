using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Authorization
{
    public sealed class Login : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Login(LocalWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact(Skip="TODO : Jon to fix")]
        public void Login_FormDisplayed()
        {
            PublicBrowsePages.CommonActions.ClickLoginLink();

            AuthorizationPages.LoginActions.EmailAddressInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.PasswordInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.LoginButtonDisplayed().Should().BeTrue();
        }

        [Fact(Skip = "TODO : Jon to fix")]
        public async Task Login_LoginSuccessful()
        {
            using var context = GetUsersContext();
            var userEmail = (await context.AspNetUsers.FirstAsync(s => s.OrganisationFunction == "Authority")).Email;

            PublicBrowsePages.CommonActions.ClickLoginLink();

            AuthorizationPages.LoginActions.Login(userEmail, DefaultPassword);

            AuthorizationPages.CommonActions.LogoutLinkDisplayed().Should().BeTrue();
        }

        [Theory(Skip ="TODO: Jon to fix")]
        [InlineData("user", "")]
        [InlineData("user", "falsePassword")]
        [InlineData("falseUser@email.com", "password")]
        [InlineData("", "password")]
        public async Task Login_UnsuccessfulLogin(string user, string password)
        {
            using var context = GetUsersContext();
            var userEmail = user == "user" ? (await context.AspNetUsers.FirstAsync(s => s.OrganisationFunction == "Authority")).Email : user;
            var userPassword = password == "password" ? DefaultPassword : password;

            PublicBrowsePages.CommonActions.ClickLoginLink();

            AuthorizationPages.LoginActions.Login(userEmail, userPassword);

            AuthorizationPages.CommonActions.LogoutLinkDisplayed().Should().BeFalse();

            AuthorizationPages.LoginActions.EmailAddressInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.PasswordInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.LoginButtonDisplayed().Should().BeTrue();
        }

        public void Dispose()
        {
            // Force logout at end of test
            Driver.Manage().Cookies.DeleteAllCookies();
        }
    }
}
