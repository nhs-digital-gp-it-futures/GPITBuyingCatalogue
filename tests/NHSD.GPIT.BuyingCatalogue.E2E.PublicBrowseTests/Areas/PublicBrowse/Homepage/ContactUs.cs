﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    public sealed class ContactUs : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ContactUs(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
               : base(
                   factory,
                   typeof(HomeController),
                   nameof(HomeController.ContactUs),
                   null,
                   testOutputHelper)
        {
        }

        [Fact]
        public void ContactUs_AllSectionsDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.PageTitle().Should().Be("Contact the NHS Buying Catalogue Team".FormatForComparison());
                CommonActions
                    .LedeText()
                    .Should()
                    .Be(("A description of the information NHS Digital collects as part of our operation of the Buying Catalogue website." +
                         "We want you to understand why we hold and process this information, and your choices.").FormatForComparison());

                CommonActions.SaveButtonDisplayed().Should().BeTrue();

                CommonActions.ElementIsDisplayed(ContactUsObjects.HomeCrumb).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ContactUsObjects.ContactUsCrumb).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ContactUsObjects.TriageSection).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ContactUsObjects.ContactMethodInput).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ContactUsObjects.FullNameInput).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ContactUsObjects.MessageInput).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ContactUsObjects.EmailAddressInput).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ContactUsObjects.PrivacyPolicyInput).Should().BeTrue();
            });
        }

        [Fact]
        public void ContactUs_ClickHomeCrumb_Redirects()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(ContactUsObjects.HomeCrumb);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.Index))
                    .Should().BeTrue();
            });
        }

        [Fact]
        public void ContactUs_ClickContactUsCrumb_Redirects()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(ContactUsObjects.ContactUsCrumb);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.ContactUs))
                    .Should().BeTrue();
            });
        }

        [Fact]
        public void ContactUs_SubmitWithoutInput_ErrorSummary()
        {
            RunTest(() =>
            {
                CommonActions.ClickSave();

                CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
                CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

                CommonActions.ElementIsDisplayed(ContactUsObjects.ContactMethodInputError).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ContactUsObjects.FullNameInputError).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ContactUsObjects.MessageInputError).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ContactUsObjects.EmailAddressInputError).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ContactUsObjects.PrivacyPolicyInputError).Should().BeTrue();
            });
        }

        [Fact]
        public void ContactUs_ValidInput_Redirects()
        {
            RunTest(() =>
            {
                TextGenerators.TextInputAddText(ContactUsObjects.FullNameInput, 20);
                TextGenerators.TextInputAddText(ContactUsObjects.MessageInput, 20);
                TextGenerators.EmailInputAddText(ContactUsObjects.EmailAddressInput, 20);

                CommonActions.ClickRadioButtonWithText("A technical fault with this website");
                CommonActions.ClickCheckboxByLabel("I have read and understood the privacy policy");

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.ContactUsConfirmation))
                    .Should().BeTrue();
            });
        }
    }
}
