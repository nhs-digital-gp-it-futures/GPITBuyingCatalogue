using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageSuppliers;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSupplier
{
    public class ManageOrganisationSupplier : PageBase
    {
        public ManageOrganisationSupplier(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddSupplierDetails()
        {
            CommonActions.ClickLinkElement(ManageSuppliers.AddASupplierLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.AddSupplierDetails))
                .Should().BeTrue();

            TextGenerators.OrganisationInputAddText(ManageSuppliers.SupplierDetailsSupplierName, 255);
            TextGenerators.OrganisationInputAddText(ManageSuppliers.SupplierDetailsSupplierLegalName, 255);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplier))
                .Should()
                .BeTrue();
        }

        public void AddSupplilerAddress()
        {
            CommonActions.ClickLinkElement(ManageSuppliers.EditSupplierAddressLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.EditSupplierAddress))
                .Should()
                .BeTrue();

            TextGenerators.TextInputAddText(ManageSuppliers.EditSupplierAddressAddressLine1, 50);
            TextGenerators.TextInputAddText(ManageSuppliers.EditSupplierAddressTown, 60);
            TextGenerators.TextInputAddText(ManageSuppliers.EditSupplierAddressPostcode, 10);
            TextGenerators.TextInputAddText(ManageSuppliers.EditSupplierAddressCountry, 60);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplier))
                .Should()
                .BeTrue();
        }

        public void AddSupplierContactDetails()
        {
            CommonActions.ClickLinkElement(ManageSuppliers.EditSupplierContactsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.ManageSupplierContacts))
                .Should()
                .BeTrue();

            CommonActions.ClickLinkElement(SupplierContactObjects.AddContact);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.AddSupplierContact)).Should().BeTrue();

            TextGenerators.FirstNameInputAddText(SupplierContactObjects.FirstNameInput, 35);
            TextGenerators.LastNameInputAddText(SupplierContactObjects.LastNameInput, 35);
            TextGenerators.OrganisationInputAddText(SupplierContactObjects.DepartmentInput, 50);
            TextGenerators.PhoneNumberInputAddText(SupplierContactObjects.PhoneNumberInput, 35);
            TextGenerators.EmailInputAddText(SupplierContactObjects.EmailInput, 255);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.ManageSupplierContacts)).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.EditSupplier)).Should().BeTrue();
        }

        public void AddSupplierStatus()
        {
            CommonActions.ClickRadioButtonWithText("Active");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.Index))
                .Should().BeTrue();
        }
    }
}
