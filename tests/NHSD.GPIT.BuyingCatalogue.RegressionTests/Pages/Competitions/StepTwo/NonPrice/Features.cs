using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;
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

            MustFeature();
            ReviewFeatureAddAnotherRequirement();
            ShouldFeature();
            ReviewFeature();
        }

        public void AddFeatureForAllNonPriceElements()
        {
            MustFeature();
            ReviewFeatureAddAnotherRequirement();
            ShouldFeature();
            ReviewFeature();
        }

        public void MustFeature()
        {
            CommonActions.ClickRadioButtonWithValue("Must");
            TextGenerators.TextInputAddText(NonPriceObjects.ElementRequirements, 100);
            CommonActions.ClickSave();
        }

        public void ReviewFeatureAddAnotherRequirement()
        {
            CommonActions.LedeText().Should().Be("Review the information you’ve provided and add any more features requirements if needed.".FormatForComparison());
            CommonActions.ClickLinkElement(NonPriceObjects.AddAnotherRequirementLink);
        }

        public void ShouldFeature()
        {
            CommonActions.ClickRadioButtonWithValue("Should");
            TextGenerators.TextInputAddText(NonPriceObjects.ElementRequirements, 100);
            CommonActions.ClickSave();
        }

        public void ReviewFeature()
        {
            CommonActions.LedeText().Should().Be("Review the information you’ve provided and add any more features requirements if needed.".FormatForComparison());
            CommonActions.ClickSave();
        }
    }
}
