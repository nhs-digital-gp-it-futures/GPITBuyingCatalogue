using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
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
                //AddBespokeMilestonesAssociatedServices(1);
                CommonActions.ClickSave();
            }
            else
            {
                //CommonActions.ClickRadioButtonWithText(AssociatedServicesBillingObjects.BespokeMilestonesAgreed);
                //CommonActions.ClickSave();
                AddBespokeMilestonesAssociatedServices(1);
                //CommonActions.ClickContinue();
            }

            //if (isDefault)
            //{
            //    CommonActions.ClickSave();
            //}
            //else
            //{
            //    CommonActions.ClickRadioButtonWithText(AssociatedServicesBillingObjects.SpecificRequirementsAgreed);
            //    CommonActions.ClickSave();
            //    CommonActions.ClickContinue();
            //}

            //    CommonActions.PageLoadedCorrectGetIndex()
            //    typeof(OrderController),
            //    nameof(OrderController.Order)).Should().BeTrue();
            }

        public void AddBespokeMilestones()
        {
            CommonActions.ClickLinkElement(ImplementationPlanObjects.ImplementationPlanAddBespokeMilestone);
            CommonActions.LedeText().Should().Be("Add an implementation plan milestone.".FormatForComparison());
            EnterMilestoneName();
            CommonActions.ClickSave();
        }

        public void AddBespokeMilestonesAssociatedServices(int value)
        {
            CommonActions.ClickLinkElement(ImplementationPlanObjects.AssociatedServicesAddBespokeMilestone);
            CommonActions.LedeText().Should().Be("Add an Associated Service milestone.".FormatForComparison());
            CommonActions.ClickDropDownListItem(value);
            EnterMilestoneName();
            TextGenerators.NumberInputAddRandomNumber(ImplementationPlanObjects.MilestoneAssociatedServiceUnits, 1, 20);
            CommonActions.ClickSave();
            CommonActions.ClickSave();
            CommonActions.ClickSave();
        }

        public void EnterMilestoneName()
        {
            TextGenerators.TextInputAddText(ImplementationPlanObjects.MileStoneName, 25);
            TextGenerators.TextInputAddText(ImplementationPlanObjects.MilestonePaymentTrigger, 50);
            CommonActions.ClickSave();
        }
    }
}
