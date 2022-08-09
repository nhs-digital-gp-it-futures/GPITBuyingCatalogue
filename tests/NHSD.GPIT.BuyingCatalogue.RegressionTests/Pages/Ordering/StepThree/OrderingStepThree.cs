using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepThree
{
    internal class OrderingStepThree : PageBase
    {
        public OrderingStepThree(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SelectImplementationPlan()
        {
            CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(OrderController),
            nameof(OrderController.Order)).Should().BeTrue();
        }

        public void SelectAssociatedServicesBilling()
        {
            CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(AssociatedServicesBillingController),
            nameof(AssociatedServicesBillingController.SpecificRequirements)).Should().BeTrue();

            CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(OrderController),
            nameof(OrderController.Order)).Should().BeTrue();
        }

        public void SelectPersonalDataProcessingInformation()
        {
            CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(OrderController),
            nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
