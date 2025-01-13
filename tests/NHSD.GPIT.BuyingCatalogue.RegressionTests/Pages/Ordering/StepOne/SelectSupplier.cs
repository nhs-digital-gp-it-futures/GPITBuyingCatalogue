using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne
{
    public class SelectSupplier : PageBase
    {
        public SelectSupplier(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SelectAndConfirmSupplier(string supplierName)
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SelectSupplier)).Should().BeTrue();

            CommonActions.AutoCompleteAddValue(SupplierObjects.SupplierAutoComplete, supplierName);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(SupplierController),
               nameof(SupplierController.ConfirmSupplier)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText("Yes");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(SupplierController),
               nameof(SupplierController.Supplier)).Should().BeTrue();
        }

        public Contact ChangeSupplierContact()
        {
            var newContact = new Contact();

            CommonActions.ClickLinkElement(SupplierObjects.CreateNewContactLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.NewContact)).Should().BeTrue();

            newContact.FirstName = TextGenerators.FirstNameInputAddText(SupplierContactInformation.FirstNameInput, 10);
            newContact.LastName = TextGenerators.LastNameInputAddText(SupplierContactInformation.LastNameInput, 10);
            newContact.Department = TextGenerators.DepartmentInputAddText(SupplierContactInformation.DepartmentInput, 10);
            newContact.Phone = TextGenerators.PhoneNumberInputAddText(SupplierContactInformation.PhoneNumberInput, 10);
            newContact.Email = TextGenerators.EmailInputAddText(SupplierContactInformation.EmailAddressInput, 20);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Summary)).Should().BeTrue();

            return newContact;
        }

        public void ConfirmSupplierForMergerAndSplit()
        {
            CommonActions.PageLoadedCorrectGetIndex(
               typeof(SupplierController),
               nameof(SupplierController.ConfirmSupplier)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText("Yes");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(SupplierController),
               nameof(SupplierController.Supplier)).Should().BeTrue();
        }
    }
}
