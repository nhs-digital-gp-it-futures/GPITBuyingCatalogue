using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Users
{
    public class ResetPassword : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int UserId = 2;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(UserId), $"{UserId}" },
        };

        public ResetPassword(LocalWebApplicationFactory factory)
            : base(factory, typeof(UsersController), nameof(UsersController.ResetPassword), Parameters)
        {
        }

        [Fact]
        public void ResetPassword_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public void ResetPassword_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();
        }

        [Fact]
        public void ResetPassword_ClickSubmitButton_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();
        }
    }
}
