using OpenQA.Selenium;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Solution_Services.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Triage;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.SolutionServices.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.SolutionServices.SolutionSelection.Price;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.SolutionServices.SolutionSelection.Quantity;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.SolutionServices.Review;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering
{
    public class OrderingPages
    {
        public OrderingPages(IWebDriver driver, CommonActions commonActions)
        {
            OrderingDashboard = new OrderingDashboard(driver, commonActions);
            OrderType = new OrderType.OrderType(driver, commonActions);
            OrderingTriage = new OrderingTriage(driver, commonActions);
            TaskList = new TaskList(driver, commonActions);
            OrderingStepOne = new OrderingStepOne(driver, commonActions);
            OrderingStepTwo = new OrderingStepTwo(driver, commonActions);
            SelectCatalogueSolution = new SelectCatalogueSolution(driver, commonActions);
            SelectCatalogueSolutionRecipients = new SelectCatalogueSolutionRecipients(driver, commonActions);
            Price = new Price(driver, commonActions);
            Quantity = new Quantity(driver, commonActions);
            ReviewSolutions = new ReviewSolutions(driver, commonActions);
        }

        internal OrderingDashboard OrderingDashboard { get; }

        internal OrderType.OrderType OrderType { get; }

        internal OrderingTriage OrderingTriage { get; }

        internal TaskList TaskList { get; }

        internal OrderingStepOne OrderingStepOne { get; }

        internal OrderingStepTwo OrderingStepTwo { get; }

        internal SelectCatalogueSolution SelectCatalogueSolution { get; }

        internal SelectCatalogueSolutionRecipients SelectCatalogueSolutionRecipients { get; }

        internal Price Price { get; }

        internal Quantity Quantity { get; }

        internal ReviewSolutions ReviewSolutions { get; }
    }
}
