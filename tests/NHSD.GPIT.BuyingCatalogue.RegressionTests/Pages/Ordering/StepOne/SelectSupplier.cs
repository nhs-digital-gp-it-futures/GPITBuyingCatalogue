using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
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
            CommonActions.AutoCompleteAddValue(SupplierObjects.SupplierAutoComplete, supplierName);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(SupplierController),
               nameof(SupplierController.SelectSupplier)).Should().BeTrue();

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
