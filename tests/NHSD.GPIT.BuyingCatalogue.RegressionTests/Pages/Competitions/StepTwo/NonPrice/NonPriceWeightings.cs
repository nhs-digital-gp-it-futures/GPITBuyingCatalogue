using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice
{
    public class NonPriceWeightings : PageBase
    {
        public NonPriceWeightings(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void Weightings(NonPriceElementType elementType)
        {
            const int weightingssingle = 100;

            switch (elementType)
            {
                case NonPriceElementType.Feature:
                    FeatureWeightings(weightingssingle);
                    break;
                case NonPriceElementType.Implementation:
                    ImplementationWeightings(weightingssingle);
                    break;
                case NonPriceElementType.Interoperability:
                    InteroperabilityWeightings(weightingssingle);
                    break;
                case NonPriceElementType.ServiceLevelAgreement:
                    ServiceLevelAgreementWeightings(weightingssingle);
                    break;
                case NonPriceElementType.All:
                    AllNonPriceElementsWeightings(weightingssingle / 4);
                    break;
                default:
                    break;
            }
        }

        public void FeatureWeightings(int weightings)
        {
            Driver.FindElement(NonPriceObjects.FeatureWeighting).SendKeys(weightings.ToString());
            CommonActions.ClickSave();
        }

        public void ImplementationWeightings(int weightings)
        {
            Driver.FindElement(NonPriceObjects.ImplementationWeighting).SendKeys(weightings.ToString());
            CommonActions.ClickSave();
        }

        public void InteroperabilityWeightings(int weightings)
        {
            Driver.FindElement(NonPriceObjects.InteroperabilityWeightings).SendKeys(weightings.ToString());
            CommonActions.ClickSave();
        }

        public void ServiceLevelAgreementWeightings(int weightings)
        {
            Driver.FindElement(NonPriceObjects.ServieLevelWeightings).SendKeys(weightings.ToString());
            CommonActions.ClickSave();
        }

        public void AllNonPriceElementsWeightings(int weightings)
        {
            Driver.FindElement(NonPriceObjects.FeatureWeighting).SendKeys(weightings.ToString());
            Driver.FindElement(NonPriceObjects.ImplementationWeighting).SendKeys(weightings.ToString());
            Driver.FindElement(NonPriceObjects.InteroperabilityWeightings).SendKeys(weightings.ToString());
            Driver.FindElement(NonPriceObjects.ServieLevelWeightings).SendKeys(weightings.ToString());
            CommonActions.ClickSave();
        }
    }
}
