using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice
{
    public class Implementation : PageBase
    {
        public Implementation(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddImplementation()
        {
            CommonActions.ClickCheckboxByLabel("Implementation");
            CommonActions.ClickSave();

            CommonActions.LedeText().Should().Be("Explain your requirements for when the winning solution is implemented.".FormatForComparison());
            TextGenerators.TextInputAddText(NonPriceObjects.ElementRequirements, 100);
            CommonActions.ClickSave();
        }

        public void AddImplementationForAllNonPriceElements()
        {
            CommonActions.LedeText().Should().Be("Explain your requirements for when the winning solution is implemented.".FormatForComparison());
            TextGenerators.TextInputAddText(NonPriceObjects.ElementRequirements, 100);
            CommonActions.ClickSave();
        }
    }
}
