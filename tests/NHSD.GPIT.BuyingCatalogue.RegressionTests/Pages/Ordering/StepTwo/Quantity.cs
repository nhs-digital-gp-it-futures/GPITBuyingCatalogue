using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo
{
    public class Quantity : PageBase
    {
        public Quantity(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void AddQuantity()
        {
            var perServiceRecipient = CommonActions.ElementIsDisplayed(ByExtensions.DataTestId("perServiceRecipient"));

            if (perServiceRecipient)
                AddPracticeListSize();
            else
                AddUnitQuantity();
        }

        public void EditQuantity(string catalogueItemName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemQuantiyLink(GetCatalogueItemID(catalogueItemName)));

            AddQuantity();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(TaskListController),
             nameof(TaskListController.TaskList)).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(OrderController),
             nameof(OrderController.Order)).Should().BeTrue();
        }

        public void AmendEditQuantity(string catalogueItemName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemQuantiyLink(GetCatalogueItemID(catalogueItemName)));

            AddQuantity();
        }

        private void AddPracticeListSize()
        {
            CommonActions.PageLoadedCorrectGetIndex(
             typeof(QuantityController),
             nameof(QuantityController.SelectServiceRecipientQuantity)).Should().BeTrue();

            var count = CommonActions.NumberOfElementsDisplayed(QuantityObjects.InputQuantityPracticeListSize);

            for (int i = 0; i < count; i++)
            {
                TextGenerators.NumberInputAddRandomNumber(QuantityObjects.InputQuantityInput(i), 50, 1000);
            }

            CommonActions.ClickSave();
        }

        private void AddUnitQuantity()
        {
            CommonActions.PageLoadedCorrectGetIndex(
             typeof(QuantityController),
             nameof(QuantityController.SelectQuantity)).Should().BeTrue();

            TextGenerators.NumberInputAddRandomNumber(QuantityObjects.QuantityInput, 50, 1000);
            CommonActions.ClickSave();
        }

        private string GetCatalogueItemID(string catalogueItemName)
        {
            using var dbContext = Factory.DbContext;

            var catalogueItem = dbContext.CatalogueItems.FirstOrDefault(i => i.Name == catalogueItemName);

            return (catalogueItem != null) ? catalogueItem.Id.ToString() : string.Empty;
        }
    }
}
