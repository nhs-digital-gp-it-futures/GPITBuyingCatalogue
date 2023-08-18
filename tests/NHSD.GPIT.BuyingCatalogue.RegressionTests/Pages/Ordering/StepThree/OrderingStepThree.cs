﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
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
            //if (isDefault)
            //{
            //CommonActions.ClickFirstRadio();
            //CommonActions.ClickSave();
            //}
            // else
            // {
            // CommonActions.ClickRadioButtonWithText(ImplementationPlanObjects.BespokeMilestonesAgreed);
            // CommonActions.ClickSave();

            //CommonActions.PageLoadedCorrectGetIndex(
            //   typeof(ImplementationPlanController),
            //   nameof(ImplementationPlanController.CustomImplementationPlan)).Should().BeTrue();

            // CommonActions.ClickContinue();
            //}

            AddBespokeMilestones();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
        nameof(OrderController.Order)).Should().BeTrue();
    }

    public void SelectAssociatedServicesBilling(bool isDefault = true)
    {
        if (isDefault)
        {
            // CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();
        }
        else
        {
            CommonActions.ClickRadioButtonWithText(AssociatedServicesBillingObjects.BespokeMilestonesAgreed);
            CommonActions.ClickSave();

            //CommonActions.PageLoadedCorrectGetIndex(
            //    typeof(ContractBillingController),
            //    nameof(ContractBillingController.BespokeBilling)).Should().BeTrue();

            CommonActions.ClickContinue();
        }

        //CommonActions.PageLoadedCorrectGetIndex(
        //       typeof(ContractBillingController),
        //       nameof(ContractBillingController.SpecificRequirements)).Should().BeTrue();

        if (isDefault)
        {
            // CommonActions.ClickFirstRadio();
            CommonActions.ClickSave();
        }
        else
        {
            CommonActions.ClickRadioButtonWithText(AssociatedServicesBillingObjects.SpecificRequirementsAgreed);
            CommonActions.ClickSave();

            //CommonActions.PageLoadedCorrectGetIndex(
            //   typeof(ContractBillingController),
            //   nameof(ContractBillingController.BespokeRequirements)).Should().BeTrue();

            CommonActions.ClickContinue();
        }

        CommonActions.PageLoadedCorrectGetIndex(
        typeof(OrderController),
        nameof(OrderController.Order)).Should().BeTrue();
    }

    public void AddBespokeMilestones()
    {
        CommonActions.ClickAddBespokeMilestone();
        CommonActions.ElementAddValue(ImplementationPlanObjects.MileStoneName, ImplementationPlanObjects.MileStoneValue);
        CommonActions.ElementAddValue(ImplementationPlanObjects.MilestonePaymentTrigger, ImplementationPlanObjects.MileStoneValue);
        CommonActions.ClickSave();
        CommonActions.ClickSave();
    }
}
}
