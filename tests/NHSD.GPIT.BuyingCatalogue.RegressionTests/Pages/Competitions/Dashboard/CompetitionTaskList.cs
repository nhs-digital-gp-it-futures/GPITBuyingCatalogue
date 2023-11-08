using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.Dashboard
{
    public class CompetitionTaskList : PageBase
    {
        public CompetitionTaskList(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CompetitionServiceRecipientsTask()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.ServiceRecipientsLink);
            CommonActions.LedeText().Should().Be("Select the organisations that will receive the winning solution for this competition or upload them using a CSV file.".FormatForComparison());

            CommonActions.ClickExpander();
        }

        public void ContractLengthTask()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.ContractLengthLink);
            CommonActions.LedeText().Should().Be("Confirm how long your contract will be in months.".FormatForComparison());
        }

        public void AwardCriteriaTask()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.AwardCriteriaLink);
            CommonActions.LedeText().Should().Be("Select if you want to use price only or price and non-price elements.".FormatForComparison());
        }

        public void CalculatePriceTask()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.CalculatePriceLink);
            CommonActions.LedeText().Should().Be("Provide information to calculate the price for each of your shortlisted solutions. The calculation will be based on the quantity you want to order and the length of the contract.".FormatForComparison());
        }

        public void ViewResult()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.ViewResultLink);
            CommonActions.LedeText().Should().Be("Review the information you’ve added for this competition. If you’re happy, you can view your results and see your winning solution.".FormatForComparison());
        }
    }
}
