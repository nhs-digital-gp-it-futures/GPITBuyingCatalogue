using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepThree
{
    internal class OrderingStepThree : PageBase
    {
        public OrderingStepThree(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
            {
            }

        public void SelectImplementationPlan(bool isDefault = true)
        {
            if (isDefault)
            {
                CommonActions.ClickSave();
            }
            else
            {
                AddBespokeMilestones();
            }

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        public void SelectAssociatedServicesBilling(bool isDefault = true)
        {
            if (isDefault)
            {
              CommonActions.ClickSave();
            }
            else
            {
                CommonActions.ClickRadioButtonWithText(AssociatedServicesBillingObjects.BespokeMilestonesAgreed);
                CommonActions.ClickSave();

                CommonActions.ClickContinue();
            }

            if (isDefault)
            {
                CommonActions.ClickSave();
            }
            else
            {
                CommonActions.ClickRadioButtonWithText(AssociatedServicesBillingObjects.SpecificRequirementsAgreed);
                CommonActions.ClickSave();
                CommonActions.ClickContinue();
            }

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(OrderController),
            nameof(OrderController.Order)).Should().BeTrue();
        }

        public void AddBespokeMilestones()
        {
            CommonActions.ClickLinkElement(ImplementationPlanObjects.ImplementationPlanAddBespokeMilestone);
            EnterMilestoneName(ImplementationPlanObjects.MileStoneValue);
            CommonActions.ClickSave();
        }

        public void EnterMilestoneName(string milestone)
        {
            CommonActions.LedeText().Should().Be("Add an implementation plan milestone.".FormatForComparison());

            Driver.FindElement(ImplementationPlanObjects.MileStoneName).SendKeys(milestone);

            TextGenerators.TextInputAddText(ImplementationPlanObjects.MileStoneName, 100);

            Driver.FindElement(ImplementationPlanObjects.MilestonePaymentTrigger).SendKeys(milestone);

            TextGenerators.TextInputAddText(ImplementationPlanObjects.MilestonePaymentTrigger, 100);

            CommonActions.ClickSave();
        }
    }
}
