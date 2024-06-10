using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions
{
    public class SolutionServiceLevelAgreement : PageBase
    {
        public SolutionServiceLevelAgreement(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddServiceLevelAgreement(string solutionId)
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.ServiceLevelAgreementLink(solutionId));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddServiceLevelAgreement))
                .Should().BeTrue();

            AddSlaType();
        }

        public void AddSlaType()
        {
            CommonActions.ClickRadioButtonWithText("Type 1 Catalogue Solution");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should().BeTrue();
        }
    }
}
