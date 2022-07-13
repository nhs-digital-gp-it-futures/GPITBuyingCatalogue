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


namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Dashboard
{
    public sealed class TaskList : PageBase
    {
        public TaskList(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void OrderDescriptionTask()
        {
            CommonActions.ClickLinkElement(OrderDashboard.OrderDescriptionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderDescriptionController),
                nameof(OrderDescriptionController.NewOrderDescription))
                    .Should().BeTrue();
        }

        public void CallOffOrderingPartyContactDetailsTask()
        {
            CommonActions.ClickLinkElement(CalloffPartyInformation.CallOffOrderingPartyContactDetailsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderingPartyController),
                nameof(OrderingPartyController.OrderingParty))
                    .Should().BeTrue();
        }

        public void SupplierInformationAndContactDetailsTask()
        {
            CommonActions.ClickLinkElement(SupplierObjects.SupplierContactDetailsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SelectSupplier))
                    .Should().BeTrue();
        }
    }
}
