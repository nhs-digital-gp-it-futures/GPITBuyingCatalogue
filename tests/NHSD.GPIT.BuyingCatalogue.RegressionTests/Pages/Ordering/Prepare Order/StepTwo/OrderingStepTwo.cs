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

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo
{
    public class OrderingStepTwo : PageBase
    {
        private const string SupplierName = "NotEmis Health";
        public OrderingStepTwo(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddSupplierInformation()
        {
            CommonActions.AutoCompleteAddValue(SupplierObjects.SupplierAutoComplete, SupplierName);
            CommonActions.ClickLinkElement(SupplierObjects.SearchResult(0));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.ConfirmSupplier)).Should().BeTrue();

            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();
        }

        public void AddSupplierContactDetails()
        {
            TextGenerators.TextInputAddText(NewContact.FirstNameInput, 20);
            TextGenerators.TextInputAddText(NewContact.LastNameInput, 20);
            TextGenerators.TextInputAddText(NewContact.DepartmentInput, 20);
            TextGenerators.TextInputAddText(NewContact.PhoneNumberInput, 20);
            TextGenerators.EmailInputAddText(NewContact.EmailInput, 50);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(OrderController),
               nameof(OrderController.Order)).Should().BeTrue();
        }

        public void TimescalesForCallOffAgreementDetails()
        {
            const int initialPeriod = 3;
            const int maximumTerm = 12;

            var date = TextGenerators.DateInputAddDateSoon(
                CommencementDate.CommencementDateDayInput,
                CommencementDate.CommencementDateMonthInput,
                CommencementDate.CommencementDateYearInput);

            CommonActions.ElementAddValue(CommencementDate.InitialPeriodInput, $"{initialPeriod}");
            CommonActions.ElementAddValue(CommencementDate.MaximumTermInput, $"{maximumTerm}");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

    }
}
