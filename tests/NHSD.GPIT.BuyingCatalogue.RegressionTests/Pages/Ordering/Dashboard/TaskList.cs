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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
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

        public void TimescalesForCallOffAgreementTask()
        {
            CommonActions.ClickLinkElement(CommencementDate.TimescalesForCallOffLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate))
                    .Should().BeTrue();
        }

        public void SelectSolutionsAndServicesTask()
        {
            CommonActions.ClickLinkElement(CatalogueSolutionObjects.SelectSolutionsAndServicesLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectSolution))
                    .Should().BeTrue();
        }

        public void SelectFundingSourcesTask()
        {
            CommonActions.ClickLinkElement(FundingSources.SelectFundingSourcesLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSources))
                    .Should().BeTrue();
        }
    }
}
