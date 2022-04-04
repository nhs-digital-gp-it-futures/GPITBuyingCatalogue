using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Supplier
{
    public sealed class NewContact : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(91002, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public NewContact(LocalWebApplicationFactory factory)
            : base(factory, typeof(SupplierController), nameof(SupplierController.NewContact), Parameters)
        {
        }

        [Fact]
        public void NewContact_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Add a contact - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.NewContact.FirstNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.NewContact.LastNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.NewContact.DepartmentInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.NewContact.PhoneNumberInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.NewContact.EmailInput).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void NewContact_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();
        }

        [Fact]
        public void NewContact_NoValues_ExpectedResult()
        {
            TextGenerators.TextInputAddText(Objects.Ordering.NewContact.FirstNameInput, 0);
            TextGenerators.TextInputAddText(Objects.Ordering.NewContact.LastNameInput, 0);
            TextGenerators.TextInputAddText(Objects.Ordering.NewContact.DepartmentInput, 0);
            TextGenerators.TextInputAddText(Objects.Ordering.NewContact.PhoneNumberInput, 0);
            TextGenerators.TextInputAddText(Objects.Ordering.NewContact.EmailInput, 0);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.NewContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.NewContact.FirstNameError,
                NewContactModelValidator.FirstNameErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.NewContact.LastNameError,
                NewContactModelValidator.LastNameErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.NewContact.EmailError,
                NewContactModelValidator.EmailErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void NewContact_EmailAddressWrongFormat_ExpectedResult()
        {
            TextGenerators.TextInputAddText(Objects.Ordering.NewContact.FirstNameInput, 20);
            TextGenerators.TextInputAddText(Objects.Ordering.NewContact.LastNameInput, 20);
            TextGenerators.TextInputAddText(Objects.Ordering.NewContact.EmailInput, 20);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.NewContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.NewContact.EmailError,
                NewContactModelValidator.EmailWrongFormatErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void NewContact_AcceptableValues_ExpectedResult()
        {
            TextGenerators.TextInputAddText(Objects.Ordering.NewContact.FirstNameInput, 20);
            TextGenerators.TextInputAddText(Objects.Ordering.NewContact.LastNameInput, 20);
            TextGenerators.EmailInputAddText(Objects.Ordering.NewContact.EmailInput, 50);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();
        }

        [Fact]
        public void NewContact_AcceptableValues_EditContact_ExpectedResult()
        {
            var firstName = TextGenerators.TextInputAddText(Objects.Ordering.NewContact.FirstNameInput, 20);
            var lastName = TextGenerators.TextInputAddText(Objects.Ordering.NewContact.LastNameInput, 20);
            var department = TextGenerators.TextInputAddText(Objects.Ordering.NewContact.DepartmentInput, 20);
            var phoneNumber = TextGenerators.TextInputAddText(Objects.Ordering.NewContact.PhoneNumberInput, 20);
            var email = TextGenerators.EmailInputAddText(Objects.Ordering.NewContact.EmailInput, 50);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();

            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.NewContact),
                Parameters);

            CommonActions.ElementIsDisplayed(Objects.Ordering.NewContact.FirstNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.NewContact.LastNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.NewContact.DepartmentInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.NewContact.PhoneNumberInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.NewContact.EmailInput).Should().BeTrue();

            CommonActions.InputValueEqualTo(Objects.Ordering.NewContact.FirstNameInput, firstName).Should().BeTrue();
            CommonActions.InputValueEqualTo(Objects.Ordering.NewContact.LastNameInput, lastName).Should().BeTrue();
            CommonActions.InputValueEqualTo(Objects.Ordering.NewContact.DepartmentInput, department).Should().BeTrue();
            CommonActions.InputValueEqualTo(Objects.Ordering.NewContact.PhoneNumberInput, phoneNumber).Should().BeTrue();
            CommonActions.InputValueEqualTo(Objects.Ordering.NewContact.EmailInput, email).Should().BeTrue();
        }
    }
}
