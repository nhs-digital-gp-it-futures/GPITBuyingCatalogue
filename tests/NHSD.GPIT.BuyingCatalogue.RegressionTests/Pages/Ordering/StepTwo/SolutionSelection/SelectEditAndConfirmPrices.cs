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
    public class SelectEditAndConfirmPrices : PageBase
    {
        private const decimal MaxPrice = 1.26M;

        public SelectEditAndConfirmPrices(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
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

        public void AmendViewAndConfirmPrice()
        {
            AmendViewCatalogueSOlutionPrice();
        }

        public void EditCatalogueSolutionPrice(string solutionName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemPriceLink(GetCatalogueSolutionID(solutionName)));

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(PricesController),
             nameof(PricesController.EditPrice)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(TaskListController),
             nameof(TaskListController.TaskList)).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(OrderController),
             nameof(OrderController.Order)).Should().BeTrue();
        }

        public void SelectEditPrice(string solutionName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemPriceLink(GetCatalogueSolutionID(solutionName)));

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(PricesController),
               nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ClickSave();
        }

        public void SelectCatalogueSolutionPrice(string solutionid)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemPriceLink(solutionid));

            if (CommonActions.GetNumberOfRadioButtonsDisplayed() > 0)
            {
                CommonActions.PageLoadedCorrectGetIndex(
                   typeof(PricesController),
                   nameof(PricesController.SelectPrice)).Should().BeTrue();

                CommonActions.ClickFirstRadio();
                CommonActions.ClickSave();
            }

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(PricesController),
               nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(TaskListController),
             nameof(TaskListController.TaskList)).Should().BeTrue();
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

        private void AmendViewCatalogueSOlutionPrice()
        {
            CommonActions.PageLoadedCorrectGetIndex(
               typeof(PricesController),
               nameof(PricesController.ViewPrice)).Should().BeTrue();

            CommonActions.ClickContinue();
        }

        private string GetCatalogueSolutionID(string solutionName)
        {
            using var dbContext = Factory.DbContext;

            var solution = dbContext.Solutions.FirstOrDefault(i => i.CatalogueItem.Name == solutionName);

            return (solution != null) ? solution.CatalogueItemId.ToString() : string.Empty;
        }
    }
}
