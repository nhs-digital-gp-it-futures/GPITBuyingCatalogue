﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.AssociatedServiceOnly
{
    public class SelectEditAndConfirmAssociatedServiceOnlyPrices : PageBase
    {
        private const decimal MaxPrice = 235.0M;

        public SelectEditAndConfirmAssociatedServiceOnlyPrices(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
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

        internal void EditPrice(string associatedServiceName)
        {
            CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditCatalogueItemPriceLink(GetAssociatedServiceID(associatedServiceName)));

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(PricesController),
             nameof(PricesController.EditPrice)).Should().BeTrue();

            TextGenerators.PriceInputAddPrice(ConfirmPriceObjects.AgreedPriceInput(0), MaxPrice);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(TaskListController),
             nameof(TaskListController.TaskList)).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(ReviewSolutionsController),
             nameof(ReviewSolutionsController.ReviewSolutions)).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(OrderController),
             nameof(OrderController.Order)).Should().BeTrue();
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

            TextGenerators.PriceInputAddPrice(ConfirmPriceObjects.AgreedPriceInput(0), MaxPrice);
            CommonActions.ClickSave();
        }

        private string GetAssociatedServiceID(string associatedServiceName)
        {
            using var dbContext = Factory.DbContext;

            var associatedService = dbContext.AssociatedServices.FirstOrDefault(i => i.CatalogueItem.Name == associatedServiceName);

            return (associatedService != null) ? associatedService.CatalogueItemId.ToString() : string.Empty;
        }
    }
}
