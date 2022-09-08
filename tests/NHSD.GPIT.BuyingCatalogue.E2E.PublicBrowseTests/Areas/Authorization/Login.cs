using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Authorization
{
    public sealed class Login : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Login(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(HomeController),
                  nameof(HomeController.Index),
                  null)
        {
        }

        [Fact]
        public void Login_FormDisplayed()
        {
            AuthorizationPages.LoginActions.EmailAddressInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.PasswordInputDisplayed().Should().BeTrue();
            AuthorizationPages.LoginActions.LoginButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task Login_LoginSuccessful()
        {
            await using var context = GetUsersContext();
            var userEmail = GetAdmin().Email;

            AuthorizationPages.LoginActions.Login(userEmail, DefaultPassword);

            AuthorizationPages.CommonActions.LogoutLinkDisplayed().Should().BeTrue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Theory]
        [InlineData("user", "")]
        [InlineData("user", "falsePassword")]
        [InlineData("falseUser@email.com", "password")]
        [InlineData("", "password")]
        public async Task Login_UnsuccessfulLogin(string user, string password)
        {
            await using var context = GetUsersContext();
            var userEmail = user == "user" ? GetAdmin().Email : user;
            var userPassword = password == "password" ? DefaultPassword : password;

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
