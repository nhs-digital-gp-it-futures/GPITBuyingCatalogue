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
                    AddAllNonPriceElements();
                    break;
                case NonPriceElementType.Multiple:
                    AddMultipleNonPriceElements();
                    break;
                default:
                    break;
            }

            var content = elementType is NonPriceElementType.All
                ? "All available non-price elements have been added for this competition."
                : "Add at least 1 optional non-price element to help you score your shortlisted solutions, for example features, implementation, interoperability or service levels.";

            CommonActions.HintText().Should().Be(content.FormatForComparison());
            CommonActions.ClickSaveAndContinue();
        }

        public void AddAllNonPriceElements()
        {
            Features.AddFeature();
            Implementation.AddImplementation();
            Interoperability.AddInteroperability();
            ServiceLevelAgreement.AddServiceLevelAgreement();
        }

        public void AddMultipleNonPriceElements()
        {
            Features.AddFeature();
            Implementation.AddImplementation();
        }
    }
}
