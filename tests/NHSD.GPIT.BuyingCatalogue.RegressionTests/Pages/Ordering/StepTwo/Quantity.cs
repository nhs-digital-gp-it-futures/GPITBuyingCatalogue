using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo
{
    public class Quantity : PageBase
    {
        public Quantity(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddQuantity()
        {
            var perServiceRecipient = CommonActions.ElementIsDisplayed(ByExtensions.DataTestId("perServiceRecipient"));

            if (perServiceRecipient)
                AddPracticeListSize();
            else
                AddUnitQuantity();
        }

        private void AddPracticeListSize()
        {
            CommonActions.PageLoadedCorrectGetIndex(
             typeof(QuantityController),
             nameof(QuantityController.SelectServiceRecipientQuantity)).Should().BeTrue();

            TextGenerators.NumberInputAddRandomNumber(QuantityObjects.InputQuantityInput(0), 50, 1000);
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
    }
}
