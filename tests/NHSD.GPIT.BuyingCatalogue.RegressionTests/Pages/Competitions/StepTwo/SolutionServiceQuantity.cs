using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo
{
    internal class SolutionServiceQuantity : PageBase
    {
        public SolutionServiceQuantity(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddSolutionQuantity(CatalogueItemId solutionId)
        {
            string catalogueSolutionId = solutionId.ToString();

            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditCatalogueItemQuantityLink(catalogueSolutionId));
            AddQuantity();
        }

        public void AddAdditionalServiceQuantity(CatalogueItemId serviceid)
        {
            string additionalServiceId = serviceid.ToString();

            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditAdditionalServiceQuantityLink(additionalServiceId));
            AddQuantity();
        }

        public void AddQuantity()
        {
            var perServiceRecipient = CommonActions.ElementIsDisplayed(ByExtensions.DataTestId("perServiceRecipient"));

            if (perServiceRecipient)
                AddPracticeListSize();
            else
                AddUnitQuantity();
        }

        private void AddPracticeListSize()
        {
            CommonActions.HintText().Should().Be("We’ve included the latest practice list sizes published by NHS Digital.".FormatForComparison());

            var count = CommonActions.NumberOfElementsDisplayed(QuantityObjects.InputQuantityPracticeListSize);

            for (int i = 0; i < count; i++)
            {
                TextGenerators.NumberInputAddRandomNumber(QuantityObjects.InputQuantityInput(i), 50, 1000);
            }

            CommonActions.ClickSave();
        }

        private void AddUnitQuantity()
        {
            TextGenerators.NumberInputAddRandomNumber(QuantityObjects.QuantityInput, 50, 1000);
            CommonActions.ClickSave();
        }
    }
}
