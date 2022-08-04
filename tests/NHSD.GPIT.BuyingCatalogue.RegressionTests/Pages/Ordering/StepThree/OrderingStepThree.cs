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

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepThree
{
    public class OrderingStepThree : PageBase
    {
        public OrderingStepThree(IWebDriver driver, CommonActions commonActions) 
            : base(driver, commonActions)
        {
        }

        public void ReviewAndCompleteOrder()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrderController),
                    nameof(OrderController.Completed))
                    .Should().BeTrue();

            CommonActions.ClickLinkElement(OrderCompletedObjects.ReturnToDashboardButton);

            CommonActions.PageLoadedCorrectGetIndex(
                   typeof(DashboardController),
                   nameof(DashboardController.Organisation))
                   .Should().BeTrue();
        }
    }
}
