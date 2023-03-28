using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class TermsOfUse :
        AnonymousTestBase,
        IDisposable
    {
        public TermsOfUse(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(TermsOfUseController),
                  nameof(TermsOfUseController.TermsOfUse))
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be("Buying Catalogue terms of use".FormatForComparison());
            CommonActions.LedeText().Should().Be("You must agree to comply with these terms of use to place orders on the Buying Catalogue.".FormatForComparison());

            CommonActions.ElementIsDisplayed(TermsOfUseObjects.SectionOne).Should().BeTrue();
            CommonActions.ElementIsDisplayed(TermsOfUseObjects.SectionTwo).Should().BeTrue();
            CommonActions.ElementIsDisplayed(TermsOfUseObjects.SectionThree).Should().BeTrue();
            CommonActions.ElementIsDisplayed(TermsOfUseObjects.SectionFour).Should().BeTrue();
            CommonActions.ElementIsDisplayed(TermsOfUseObjects.SectionFive).Should().BeTrue();
            CommonActions.ElementIsDisplayed(TermsOfUseObjects.SectionSix).Should().BeTrue();
            CommonActions.ElementIsDisplayed(TermsOfUseObjects.SectionSeven).Should().BeTrue();
            CommonActions.ElementIsDisplayed(TermsOfUseObjects.SectionEight).Should().BeTrue();
            CommonActions.ElementIsDisplayed(TermsOfUseObjects.SectionNine).Should().BeTrue();
            CommonActions.ElementIsDisplayed(TermsOfUseObjects.SectionTen).Should().BeTrue();
            CommonActions.ElementIsDisplayed(TermsOfUseObjects.SectionEleven).Should().BeTrue();
        }

        [Fact]
        public void Unauthenticated_NoFormVisible()
        {
            NavigateToUrl(
                typeof(AccountController),
                nameof(AccountController.Logout));

            NavigateToUrl(
                typeof(TermsOfUseController),
                nameof(TermsOfUseController.TermsOfUse));

            CommonActions.ElementIsDisplayed(TermsOfUseObjects.Form).Should().BeFalse();
        }

        [Fact]
        public void Authenticated_HasAccepted_FormVisible()
        {
            Login();

            CommonActions.ElementIsDisplayed(TermsOfUseObjects.Form).Should().BeTrue();
            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(1);

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void Authenticated_HasNotAccepted_FormVisible()
        {
            ClearPrivacyPolicySelection();

            CommonActions.ElementIsDisplayed(TermsOfUseObjects.Form).Should().BeTrue();
            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(3);

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void Authenticated_Admin_NoFormVisible()
        {
            LoginAsAdmin();

            CommonActions.ElementIsDisplayed(TermsOfUseObjects.Form).Should().BeFalse();
        }

        [Fact]
        public void Submit_NoSelection_ThrowsError()
        {
            ClearPrivacyPolicySelection();

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        [Fact]
        public void Submit_Valid_UpdatesUser()
        {
            ClearPrivacyPolicySelection();

            CommonActions.ClickCheckboxByLabel("I have read and agree to comply with these terms of use");
            CommonActions.ClickCheckboxByLabel("I have read and understood the privacy policy (opens in a new tab)");
            CommonActions.ClickSave();

            using var context = GetEndToEndDbContext();
            var updatedUser = context.AspNetUsers.AsNoTracking().First(u => u.Email == UserSeedData.AliceEmail);
            updatedUser.AcceptedTermsOfUseDate.Should().NotBeNull();
        }

        [Fact]
        public void ClickPrivacyAndCookiesPolicy_NavigatesCorrectly()
        {
            CommonActions.ClickLinkElement(TermsOfUseObjects.PrivacyAndCookiesPolicyLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.PrivacyPolicy)).Should().BeTrue();
        }

        [Fact]
        public void ClickHomepageLink_NavigatesCorrectly()
        {
            CommonActions.ClickLinkElement(TermsOfUseObjects.HomepageLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void ClickHomeCrumb_NavigatesCorrectly()
        {
            CommonActions.ClickLinkElement(TermsOfUseObjects.HomeCrumb);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void OrderDashboard_HasNotAccepted_RedirectToTerms()
        {
            ClearPrivacyPolicySelection();

            CommonActions.ClickLinkElement(By.LinkText("Create or manage orders"));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TermsOfUseController),
                nameof(TermsOfUseController.TermsOfUse)).Should().BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.First(u => u.Email == UserSeedData.AliceEmail);
            user.AcceptedTermsOfUseDate = DateTime.UtcNow;

            context.SaveChanges();
        }

        private void ClearPrivacyPolicySelection()
        {
            using var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.First(u => u.Email == UserSeedData.AliceEmail);
            user.AcceptedTermsOfUseDate = null;

            context.SaveChanges();

            Login();
        }

        private void Login()
        {
            NavigateToUrl(
                typeof(AccountController),
                nameof(AccountController.Logout));

            NavigateToUrl(
                    typeof(AccountController),
                    nameof(AccountController.Login));

            BuyerLogin(UserSeedData.AliceEmail);

            NavigateToUrl(
                typeof(TermsOfUseController),
                nameof(TermsOfUseController.TermsOfUse));
        }

        private void LoginAsAdmin()
        {
            NavigateToUrl(
                typeof(AccountController),
                nameof(AccountController.Logout));

            NavigateToUrl(
                    typeof(AccountController),
                    nameof(AccountController.Login));

            AuthorityLogin();

            NavigateToUrl(
                typeof(TermsOfUseController),
                nameof(TermsOfUseController.TermsOfUse));
        }
    }
}
