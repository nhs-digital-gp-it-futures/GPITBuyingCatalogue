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

        public void AddCompetitionServiceRecipient(ServiceRecipientSelectionMode recipients)
        {
            switch (recipients)
            {
                case ServiceRecipientSelectionMode.All:
                    CommonActions.ClickAllCheckboxes();
                    break;
                case ServiceRecipientSelectionMode.Single:
                    CommonActions.ClickFirstCheckbox();
                    break;
                case ServiceRecipientSelectionMode.Multiple:
                    CommonActions.ClickMultipleCheckboxes(new Random().Next(2, 10));
                    break;
                case ServiceRecipientSelectionMode.None:
                default:
                    break;
            }

            CommonActions.ClickSave();
        }

        public void ConfirmServiceReceipientsChanges()
        {
            CommonActions.LedeText().Should().Be("Review the organisations you’ve selected to receive the winning solution for this competition.".FormatForComparison());

            CommonActions.ClickSave();
        }
    }
}
