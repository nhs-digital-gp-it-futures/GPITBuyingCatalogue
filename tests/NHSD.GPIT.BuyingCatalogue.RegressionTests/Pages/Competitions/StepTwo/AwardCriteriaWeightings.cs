using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo
{
    internal class AwardCriteriaWeightings : PageBase
    {
        public AwardCriteriaWeightings(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void PriceNonPriceAwardCriteriaWeightings()
        {
            int priceOnlyweightings;
            int priceAndNonPriceweightings;

            priceOnlyweightings = GeneratePriceWeightings();
            priceAndNonPriceweightings = GenerateNonPriceWeightings(priceOnlyweightings);

            Driver.FindElement(CreateCompetitionObjects.PriceInput).SendKeys(priceOnlyweightings.ToString());
            Driver.FindElement(CreateCompetitionObjects.NonPriceInput).SendKeys(priceAndNonPriceweightings.ToString());

            CommonActions.ClickSave();
            CommonActions.LedeText().Should().Be("Complete the following steps to carry out a competition.".FormatForComparison());
        }

        private int GeneratePriceWeightings()
        {
            Random random = new Random();
            int priceWeighting = random.Next(13, 19) * 5;

            return priceWeighting;
        }

        private int GenerateNonPriceWeightings(int priceOnlyweightings)
        {
            int nonPriceWeightings = 100 - priceOnlyweightings;
            return nonPriceWeightings;
        }
    }
}
