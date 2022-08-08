using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class SelectAndConfirmAdditionalServicePrice : PageBase
    {
        private const decimal MaxPrice = 0.07M;

        public SelectAndConfirmAdditionalServicePrice(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SelectAndConfirmPrice()
        {
            SelectPrice();
            ConfirmPrice();
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
    }
}
