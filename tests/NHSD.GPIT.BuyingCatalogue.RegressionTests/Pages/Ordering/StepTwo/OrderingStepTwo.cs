using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.FundingSource;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo
{
    public class OrderingStepTwo : PageBase
    {
        public OrderingStepTwo(IWebDriver driver, CommonActions commonActions) 
            : base(driver, commonActions)
        {
        }

        public void AddFundingSources()
        {
            CommonActions.ClickLinkElement(FundingSources.EditLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSource))
                .Should().BeTrue();

            if (CommonActions.GetNumberOfRadioButtonsDisplayed() > 0)
            {
                CommonActions.ClickFirstRadio();
            }

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSources))
                .Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(OrderController),
              nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
