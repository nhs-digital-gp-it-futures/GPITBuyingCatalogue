using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice
{
    public class NonPriceElements : PageBase
    {
        public NonPriceElements(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddNonPriceElements(NonPriceElementType elementType)
        {
            AddElements();
            if (elementType == NonPriceElementType.Feature)
            {
                AddFeature();
            }
        }

        public void AddElements()
        {
            CommonActions.ClickLinkElement(NonPriceObjects.AddNonPriceElementLink);
            CommonActions.LedeText().Should().Be("You can add any or all optional non-price elements to help you score your shortlisted solutions.".FormatForComparison());
        }

        public void AddFeature()
        {
            CommonActions.ClickCheckboxByLabel("Features");
            CommonActions.ClickSave();

            CommonActions.LedeText().Should().Be("Explain your requirements for features provided by the winning solution. You can add more features requirements later if needed.".FormatForComparison());

            CommonActions.ClickRadioButtonWithValue("Must");
            TextGenerators.TextInputAddText(NonPriceObjects.FeatureRequirements, 100);
            CommonActions.ClickSave();
        }
    }
}
