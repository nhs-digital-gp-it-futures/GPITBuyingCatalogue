using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
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
            MustFeature();
            ShouldFeature();
        }

        private void MustFeature()
        {
            AddFeatureRequirement();
            CommonActions.ClickRadioButtonWithValue("Must");
            TextGenerators.TextInputAddText(NonPriceObjects.ElementRequirements, 100);
            CommonActions.ClickSave();
        }

        private void ShouldFeature()
        {
            AddFeatureRequirement();
            CommonActions.ClickRadioButtonWithValue("Should");
            TextGenerators.TextInputAddText(NonPriceObjects.ElementRequirements, 100);
            CommonActions.ClickSave();
        }

        private void AddFeatureRequirement()
        {
            CommonActions.ClickLinkElement(NonPriceObjects.AddFeaturesLink);
            CommonActions.HintText().Should().Be("Explain your requirements for features provided by the winning solution. You can add more features requirements later if needed.".FormatForComparison());
        }
    }
}
