using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice
{
    public class Interoperability : PageBase
    {
        public Interoperability(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddInteroperability()
        {
            CommonActions.ClickCheckboxByLabel("Interoperability");
            CommonActions.ClickSave();

            CommonActions.LedeText().Should().Be("Select which integrations your winning solution needs to work with.".FormatForComparison());
            CommonActions.ClickAllCheckboxes();
            CommonActions.ClickSave();
        }

        public void AddInteroperabilityForAllNonPriceElements()
        {
            CommonActions.LedeText().Should().Be("Select which integrations your winning solution needs to work with.".FormatForComparison());
            CommonActions.ClickAllCheckboxes();
            CommonActions.ClickSave();
        }
    }
}
