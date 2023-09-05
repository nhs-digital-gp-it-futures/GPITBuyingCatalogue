using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOne.SolutionSelection
{
    public class CompetitionServiceRecipients : PageBase
    {
        public CompetitionServiceRecipients(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddCompetitionServiceRecipient(int multipleServiceRecipients, bool allServiceRecipients)
        {
            if (multipleServiceRecipients > 0 && !allServiceRecipients)
                CommonActions.ClickMultipleCheckboxes(multipleServiceRecipients);
            else if (multipleServiceRecipients == 0 && allServiceRecipients)
                CommonActions.ClickAllCheckboxes();
            else
                CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();
        }

        public void ConfirmServiceReceipientsChanges()
        {
            CommonActions.LedeText().Should().Be("Review the organisations you’ve selected to receive the winning solution for this competition.".FormatForComparison());

            CommonActions.ClickSave();
        }
    }
}
