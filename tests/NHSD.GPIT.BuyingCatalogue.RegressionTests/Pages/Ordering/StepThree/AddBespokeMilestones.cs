using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepThree
{
    public class AddBespokeMilestones : PageBase
    {
        public AddBespokeMilestones(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CatalogueSolutionAddBespokeMilestone()
        {
            CommonActions.ClickLinkElement(ImplementationPlanObjects.ImplementationPlanAddBespokeMilestone);
            CommonActions.HintText().Should().Be("Add an implementation plan milestone.".FormatForComparison());

            CatalogueSolutionEnterMilestoneName();
            CommonActions.ClickSave();
        }

        public void CatalogueSolutionEnterMilestoneName()
        {
            TextGenerators.TextInputAddText(ImplementationPlanObjects.MileStoneName, 25);
            TextGenerators.TextInputAddText(ImplementationPlanObjects.MilestonePaymentTrigger, 50);

            CommonActions.ClickSave();
        }

        public void AssociatedServicesAddBespokeMilestones(int value)
        {
            CommonActions.ClickLinkElement(ImplementationPlanObjects.AssociatedServicesAddBespokeMilestone);
            CommonActions.HintText().Should().Be("Add an Associated Service milestone.".FormatForComparison());
            CommonActions.ClickDropDownListItem(value);
            AssociatedServicesEnterMilestoneName();

            CommonActions.HintText().Should().Be("Review the default milestones that will act as payment triggers and create bespoke ones if required.".FormatForComparison());
            CommonActions.ClickSave();

            CommonActions.HintText().Should().Be("Provide details of any specific requirements for your Associated Services.".FormatForComparison());
            CommonActions.ClickSave();
        }

        public void AssociatedServicesEnterMilestoneName()
        {
            TextGenerators.TextInputAddText(ImplementationPlanObjects.MileStoneName, 25);
            TextGenerators.TextInputAddText(ImplementationPlanObjects.MilestonePaymentTrigger, 50);
            TextGenerators.NumberInputAddRandomNumber(ImplementationPlanObjects.MilestoneAssociatedServiceUnits, 1, 20);

            CommonActions.ClickSave();
        }
    }
}
