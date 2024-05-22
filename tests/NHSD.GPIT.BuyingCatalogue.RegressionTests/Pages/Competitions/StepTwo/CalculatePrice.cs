using FluentAssertions;
using Microsoft.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
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
            string catalogueSolutionId = solutionId.ToString();

            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditCompetitionSolutionLink(catalogueSolutionId));
            CommonActions.HintText().Should().Be("Provide information to calculate the price for this shortlisted solution and any Additional or Associated Services you’ll need.".FormatForComparison());
        }

        public void SelectPrice(CatalogueItemId solutionId)
        {
            string catalogueSolutionId = solutionId.ToString();

            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditCompetitionSolutionLink(catalogueSolutionId));
            CommonActions.HintText().Should().Be("Provide information to calculate the price for this shortlisted solution and any Additional or Associated Services you’ll need.".FormatForComparison());

            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditCompetitionSolutionLink(catalogueSolutionId));

            if (CommonActions.GetNumberOfRadioButtonsDisplayed() > 0)
            {
                CommonActions.PageLoadedCorrectGetIndex(
                   typeof(CompetitionHubController),
                   nameof(CompetitionHubController.SelectPrice)).Should().BeTrue();

                CommonActions.ClickFirstRadio();
                CommonActions.ClickSave();

                CommonActions.HintText().Should().Be("Confirm the price you'll be paying. We've included the list price, but this can be changed if you’ve agreed a different rate with the supplier.".FormatForComparison());

                CommonActions.ClickSave();
            }
        }

        public void SelectAdditionalServicePrice(CatalogueItemId serviceId)
        {
            string additionalServiceId = serviceId.ToString();
            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditAdditionalServiceWithTieredPriceEditLink(additionalServiceId));

            if (CommonActions.GetNumberOfRadioButtonsDisplayed() > 0)
            {
                CommonActions.PageLoadedCorrectGetIndex(
                   typeof(CompetitionHubController),
                   nameof(CompetitionHubController.SelectPrice)).Should().BeTrue();

                CommonActions.ClickFirstRadio();
                CommonActions.ClickSave();

                CommonActions.HintText().Should().Be("Confirm the price you'll be paying. We've included the list price, but this can be changed if you’ve agreed a different rate with the supplier.".FormatForComparison());

                CommonActions.ClickSave();
            }
        }

        public void ConfirmSolutionPrice(CatalogueItemId solutionId)
        {
            int catalogueSolutionServicePriceId = GetCatalogueItemWithPrices(solutionId);
            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditCatalogueItemPriceIdLink(catalogueSolutionServicePriceId));
            CommonActions.HintText().Should().Be("Confirm the price you'll be paying. We've included the list price, but this can be changed if you’ve agreed a different rate with the supplier.".FormatForComparison());

            CommonActions.ClickSave();
        }

        public void ConfirmAdditionalServicePrice(CatalogueItemId serviceId)
        {
            int catalogueServicePriceId = GetCatalogueItemWithPrices(serviceId);
            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditAdditionalServiceEditPriceLink(catalogueServicePriceId));
            CommonActions.HintText().Should().Be("Confirm the price you'll be paying. We've included the list price, but this can be changed if you’ve agreed a different rate with the supplier.".FormatForComparison());

            CommonActions.ClickSave();
        }

        public void ConfirmPriceAndQuantity()
        {
            CommonActions.ClickSaveAndContinue();
            CommonActions.HintText().Should().Be("Provide information to calculate the price for each of your shortlisted solutions. The calculation will be based on the quantity you want to order and the length of the contract.".FormatForComparison());
        }

        public void ConfirmCalculatePrice()
        {
            CommonActions.ClickSaveAndContinue();
            CommonActions.HintText().Should().Be("Complete the following steps to carry out a competition.".FormatForComparison());
        }

        private int GetCatalogueItemWithPrices(CatalogueItemId id)
        {
            using var dbContext = Factory.DbContext;

            var catalogueservicepriceid = dbContext.CataloguePrices
                .Where(p => p.CatalogueItemId == id)
                .Select(x => x.CataloguePriceId).FirstOrDefault();

            return catalogueservicepriceid;
        }
    }
}
