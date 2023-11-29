using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice
{
    public class Features : PageBase
    {
        public Features(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddFeature()
        {
            CommonActions.ClickCheckboxByLabel("Features");
            CommonActions.ClickSave();

            CommonActions.LedeText().Should().Be("Explain your requirements for features provided by the winning solution. You can add more features requirements later if needed.".FormatForComparison());

            CommonActions.ClickRadioButtonWithValue("Must");
            TextGenerators.TextInputAddText(NonPriceObjects.FeatureRequirements, 100);
            CommonActions.ClickSave();

            CommonActions.LedeText().Should().Be("Review the information you’ve provided and add any more features requirements if needed.".FormatForComparison());
            CommonActions.ClickLinkElement(NonPriceObjects.AddAnotherRequirementLink);

            CommonActions.ClickRadioButtonWithValue("Should");
            TextGenerators.TextInputAddText(NonPriceObjects.FeatureRequirements, 100);
            CommonActions.ClickSave();

            CommonActions.LedeText().Should().Be("Review the information you’ve provided and add any more features requirements if needed.".FormatForComparison());
            CommonActions.ClickSave();

            CommonActions.LedeText().Should().Be("Add at least 1 optional non-price element to help you score your shortlisted solutions, for example features, implementation, interoperability or service levels.".FormatForComparison());
            CommonActions.ClickContinue();
        }
    }
}
