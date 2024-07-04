using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice
{
    public class CompetitionNonPriceElements : PageBase
    {
        public CompetitionNonPriceElements(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
            Features = new Features(driver, commonActions);
            Implementation = new Implementation(driver, commonActions);
            Interoperability = new Interoperability(driver, commonActions);
            ServiceLevelAgreement = new ServiceLevelAgreement(driver, commonActions);
        }

        public Features Features { get; }

        public Implementation Implementation { get; }

        public Interoperability Interoperability { get; }

        public ServiceLevelAgreement ServiceLevelAgreement { get; }

        public void AddNonPriceElements(NonPriceElementType elementType)
        {
            AddElements();

            switch (elementType)
            {
                case NonPriceElementType.Feature:
                    Features.AddFeature();
                    break;
                case NonPriceElementType.Implementation:
                    Implementation.AddImplementation();
                    break;
                case NonPriceElementType.Interoperability:
                    Interoperability.AddInteroperability();
                    break;
                case NonPriceElementType.ServiceLevelAgreement:
                    ServiceLevelAgreement.AddServiceLevelAgreement();
                    break;
                case NonPriceElementType.All:
                    SelectAllNonPriceElements();
                    AddAllNonPriceElements();
                    break;
                case NonPriceElementType.Multiple:
                    SelectMultipleNonPriceElements();
                    AddMultipleNonPriceElements();
                    break;
                default:
                    break;
            }
        }

        public void AddElements()
        {
            CommonActions.ClickLinkElement(NonPriceObjects.AddNonPriceElementLink);
            CommonActions.HintText().Should().Be("You can add any or all optional non-price elements to help you score your shortlisted solutions.".FormatForComparison());
        }

        public void AddNonPriceElement()
        {
                CommonActions.HintText().Should().Be("Add at least 1 optional non-price element to help you score your shortlisted solutions, for example features, implementation, interoperability or service levels.".FormatForComparison());
                CommonActions.ClickSaveAndContinue();
        }

        public void AllNonPriceElementsReview()
        {
            CommonActions.HintText().Should().Be("All available non-price elements have been added for this competition.".FormatForComparison());
            CommonActions.ClickSaveAndContinue();
        }

        public void SelectAllNonPriceElements()
        {
            CommonActions.ClickCheckboxByLabel("Features");
            CommonActions.ClickCheckboxByLabel("Implementation");
            CommonActions.ClickCheckboxByLabel("Interoperability");
            CommonActions.ClickCheckboxByLabel("Service levels");
            CommonActions.ClickSave();
        }

        public void AddAllNonPriceElements()
        {
            Features.AddFeatureForAllNonPriceElements();
            Implementation.AddImplementationForAllNonPriceElements();
            Interoperability.AddInteroperabilityForAllNonPriceElements();
            ServiceLevelAgreement.AddServiceLevelAgreementForAllNonPriceElements();
        }

        public void SelectMultipleNonPriceElements()
        {
            CommonActions.ClickCheckboxByLabel("Features");
            CommonActions.ClickCheckboxByLabel("Implementation");
            CommonActions.ClickSave();
        }

        public void AddMultipleNonPriceElements()
        {
            Features.AddFeatureForAllNonPriceElements();
            Implementation.AddImplementationForAllNonPriceElements();
        }
    }
}
