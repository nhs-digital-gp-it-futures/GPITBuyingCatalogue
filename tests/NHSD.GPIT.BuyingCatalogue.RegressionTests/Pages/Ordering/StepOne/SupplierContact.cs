using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne
{
    public class SupplierContact : PageBase
    {
        public SupplierContact(IWebDriver driver, CommonActions commonActions)
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
            CommonActions.ClickLinkElement(NewContact.AddNewContactLink);

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(SupplierController),
               nameof(SupplierController.NewContact)).Should().BeTrue();

            TextGenerators.TextInputAddText(NewContact.FirstNameInput, 10);
            TextGenerators.TextInputAddText(NewContact.LastNameInput, 10);
            TextGenerators.TextInputAddText(NewContact.DepartmentInput, 10);
            TextGenerators.TextInputAddText(NewContact.PhoneNumberInput, 10);
            TextGenerators.TextInputAddText(NewContact.EmailInput, 10);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();

            CommonActions.IsRadioButtonChecked("-1");
        }
    }
}
