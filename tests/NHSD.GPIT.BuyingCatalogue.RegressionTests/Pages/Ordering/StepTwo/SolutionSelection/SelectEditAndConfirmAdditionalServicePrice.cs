using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class SelectEditAndConfirmAdditionalServicePrice : PageBase
    {
        private const decimal MaxPrice = 0.07M;

        public SelectEditAndConfirmAdditionalServicePrice(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void SelectAndConfirmPrice()
        {
            SelectPrice();
            ConfirmPrice();
        }

        public void EditConfirmPrice()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(PricesController),
                 nameof(PricesController.EditPrice)).Should().BeTrue();

            CommonActions.ClickSave();
        }

        public void EditAdditionalServicePrice(string additionalServiceName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemPriceLink(GetAdditionalServiceID(additionalServiceName)));

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(PricesController),
             nameof(PricesController.EditPrice)).Should().BeTrue();

            TextGenerators.PriceInputAddPrice(PriceObjects.AgreedPriceInput(0), MaxPrice);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(TaskListController),
             nameof(TaskListController.TaskList)).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(OrderController),
             nameof(OrderController.Order)).Should().BeTrue();
        }

        public void SelectEditAdditionalServicePrice(string additionalServiceName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemPriceLink(GetAdditionalServiceID(additionalServiceName)));

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(PricesController),
               nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ClickSave();
        }

        private void SelectPrice()
        {
            if (CommonActions.GetNumberOfRadioButtonsDisplayed() > 0)
            {
                CommonActions.PageLoadedCorrectGetIndex(
                   typeof(PricesController),
                   nameof(PricesController.SelectPrice)).Should().BeTrue();

                CommonActions.ClickFirstRadio();
                CommonActions.ClickSave();
            }
        }

        private void ConfirmPrice()
        {
            CommonActions.PageLoadedCorrectGetIndex(
               typeof(PricesController),
               nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            TextGenerators.PriceInputAddPrice(PriceObjects.AgreedPriceInput(0), MaxPrice);
            CommonActions.ClickSave();
        }

        private string GetAdditionalServiceID(string additionalServiceName)
        {
            using var db = Factory.DbContext;

            var additionalService = db.AdditionalServices.FirstOrDefault(a => a.CatalogueItem.Name == additionalServiceName);

            return (additionalService != null) ? additionalService.CatalogueItemId.ToString() : string.Empty;
        }
    }
}
