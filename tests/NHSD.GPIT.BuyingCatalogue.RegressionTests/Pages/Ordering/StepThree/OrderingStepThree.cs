using CsvHelper;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepThree
{
    public class OrderingStepThree : PageBase
    {
        public OrderingStepThree(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
            AddBespokeMilestones = new AddBespokeMilestones(driver, commonActions);
        }

        public AddBespokeMilestones AddBespokeMilestones { get; }

        public void SelectImplementationPlan(bool isDefault = true)
        {
            if (isDefault)
            {
                CommonActions.ClickSave();
            }
            else
            {
                AddBespokeMilestones.CatalogueSolutionAddBespokeMilestone();
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
                CommonActions.HintText().Should().Be("Provide details of any specific requirements for your Associated Services.".FormatForComparison());
                CommonActions.ClickSave();
            }
            else
            {
                AddBespokeMilestones.AssociatedServicesAddBespokeMilestones(1);
            }
        }
    }
}
