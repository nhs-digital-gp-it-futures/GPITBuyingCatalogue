using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo
{
    public class OrderingStepTwo : PageBase
    {
        private const string SolutionName = "NotEmis Web GP";
        private const decimal MaxPrice = 500.49M;
        private const bool HasAssociatedServices = false;

        public OrderingStepTwo(IWebDriver driver, CommonActions commonActions) 
            : base(driver, commonActions)
        {
        }

        public void SelectSolution(bool withAdditionalService = false)
        {
            CommonActions.ClickRadioButtonWithText(SolutionName);

            if (withAdditionalService is true)
            {
                // TO DO
            }

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(ServiceRecipientsController),
               nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(PricesController),
               nameof(PricesController.SelectPrice)).Should().BeTrue();

            CommonActions.ClickFirstRadio();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(PricesController),
               nameof(PricesController.ConfirmPrice)).Should().BeTrue();

            TextGenerators.PriceInputAddPrice(ConfirmPriceObjects.AgreedPriceInput(0), MaxPrice);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(QuantityController),
              nameof(QuantityController.SelectServiceRecipientQuantity)).Should().BeTrue();

            TextGenerators.NumberInputAddRandomNumber(QuantityObjects.InputQuantityInput(0), 50, 1000);
            CommonActions.ClickSave();

            if (HasAssociatedServices)
            {
                CommonActions.PageLoadedCorrectGetIndex(
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.AddAssociatedServices)).Should().BeTrue();
                //TO DO -> adding the "yes" path for adding an associated service
            }

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(ReviewSolutionsController),
              nameof(ReviewSolutionsController.ReviewSolutions)).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(OrderController),
              nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
