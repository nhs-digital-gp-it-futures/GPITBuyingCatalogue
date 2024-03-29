﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne
{
    public class SupplierContacts : PageBase
    {
        public SupplierContacts(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void ConfirmContact(bool addNewContact)
        {
            if (CommonActions.GetNumberOfRadioButtonsDisplayed() > 0)
            {
                CommonActions.ClickFirstRadio();
            }

            if (addNewContact)
            {
                AddNewContact();
            }

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(OrderController),
               nameof(OrderController.Order)).Should().BeTrue();
        }

        public void AddNewContact()
        {
            CommonActions.ClickLinkElement(NewContactObjects.AddNewContactLink);

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(SupplierController),
               nameof(SupplierController.NewContact)).Should().BeTrue();

            TextGenerators.FirstNameInputAddText(NewContactObjects.FirstNameInput, 10);
            TextGenerators.LastNameInputAddText(NewContactObjects.LastNameInput, 10);
            TextGenerators.TextInputAddText(NewContactObjects.DepartmentInput, 10);
            TextGenerators.PhoneNumberInputAddText(NewContactObjects.PhoneNumberInput, 10);
            TextGenerators.EmailInputAddText(NewContactObjects.EmailInput, 10);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();

            CommonActions.IsRadioButtonChecked("-1");
        }
    }
}
