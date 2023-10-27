using FluentAssertions;
using Microsoft.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Quantity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo
{
    public class CalculatePrice : PageBase
    {
        public CalculatePrice(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void SolutionPrice(CatalogueItemId solutionId)
        {
            string solutionid = solutionId.ToString();

            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditCompetitionSolutionLink(solutionid));
            CommonActions.LedeText().Should().Be("Provide information to calculate the price for this shortlisted solution and any Additional or Associated Services you’ll need.".FormatForComparison());
        }

        public void SelectPrice(CatalogueItemId solutionId)
        {
            string solutionid = solutionId.ToString();

            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditCompetitionSolutionLink(solutionid));
            CommonActions.LedeText().Should().Be("Provide information to calculate the price for this shortlisted solution and any Additional or Associated Services you’ll need.".FormatForComparison());

            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditCompetitionSolutionLink(solutionid));

            if (CommonActions.GetNumberOfRadioButtonsDisplayed() > 0)
            {
                CommonActions.PageLoadedCorrectGetIndex(
                   typeof(CompetitionHubController),
                   nameof(CompetitionHubController.SelectPrice)).Should().BeTrue();

                CommonActions.ClickFirstRadio();
                CommonActions.ClickSave();

                CommonActions.LedeText().Should().Be("Confirm the price you'll be paying. We've included the list price, but this can be changed if you’ve agreed a different rate with the supplier.".FormatForComparison());

                CommonActions.ClickSave();
            }
        }

        public void SelectAdditionalServicePrice(CatalogueItemId serviceid)
        {
            string additionalserviceid = serviceid.ToString();
            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditAdditionalServiceWithTieredPriceEditLink(additionalserviceid));

            if (CommonActions.GetNumberOfRadioButtonsDisplayed() > 0)
            {
                CommonActions.PageLoadedCorrectGetIndex(
                   typeof(CompetitionHubController),
                   nameof(CompetitionHubController.SelectPrice)).Should().BeTrue();

                CommonActions.ClickFirstRadio();
                CommonActions.ClickSave();

                CommonActions.LedeText().Should().Be("Confirm the price you'll be paying. We've included the list price, but this can be changed if you’ve agreed a different rate with the supplier.".FormatForComparison());

                CommonActions.ClickSave();
            }
        }

        public void ConfirmSolutionPrice(CatalogueItemId solutionId)
        {
            int cataloguesolutionservicepriceid = GetCatalogueItemWithPrices(solutionId);
            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditCatalogueItemPriceIdLink(cataloguesolutionservicepriceid));
            CommonActions.LedeText().Should().Be("Confirm the price you'll be paying. We've included the list price, but this can be changed if you’ve agreed a different rate with the supplier.".FormatForComparison());

            CommonActions.ClickSave();
        }

        public void ConfirmAdditionalServicePrice(CatalogueItemId serviceid)
        {
            int catalogueservicepriceid = GetCatalogueItemWithPrices(serviceid);
            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditAdditionalServiceEditPriceLink(catalogueservicepriceid));
            CommonActions.LedeText().Should().Be("Confirm the price you'll be paying. We've included the list price, but this can be changed if you’ve agreed a different rate with the supplier.".FormatForComparison());

            CommonActions.ClickSave();
        }

        public void ConfirmPriceAndQuantity()
        {
            CommonActions.ClickContinue();
            CommonActions.LedeText().Should().Be("Provide information to calculate the price for each of your shortlisted solutions. The calculation will be based on the quantity you want to order and the length of the contract.".FormatForComparison());
        }

        public void AddSolutionQuantity(CatalogueItemId solutionId)
        {
            string solutionid = solutionId.ToString();

            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditCatalogueItemQuantityLink(solutionid));
            AddQuantity();
        }

        public void AddAdditionalServiceQuantity(CatalogueItemId serviceid)
        {
            string additionalserviceid = serviceid.ToString();

            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditAdditionalServiceQuantityLink(additionalserviceid));
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

        public void ConfirmCalculatePrice()
        {
            CommonActions.ClickContinue();
            CommonActions.LedeText().Should().Be("Complete the following steps to carry out a competition.".FormatForComparison());
        }

        private int GetCatalogueItemWithPrices(CatalogueItemId id)
        {
            using var dbContext = Factory.DbContext;

            var catalogueservicepriceid = dbContext.CataloguePrices
                .Where(p => p.CatalogueItemId == id)
                .Select(x => x.CataloguePriceId).FirstOrDefault();

            return catalogueservicepriceid;
        }

        private void AddPracticeListSize()
        {
            CommonActions.LedeText().Should().Be("We’ve included the latest practice list sizes published by NHS Digital.".FormatForComparison());

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
