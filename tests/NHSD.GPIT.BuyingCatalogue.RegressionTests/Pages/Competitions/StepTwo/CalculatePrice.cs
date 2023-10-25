using CsvHelper;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
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
            //var priceid = GetCatalogueItemWithPrices(solutionId);

            string solutionid = solutionId.ToString();

            CommonActions.ClickLinkElement(PriceAndQuantityObjects.EditCompetitionSolutionLink(solutionid));
            CommonActions.LedeText().Should().Be("Provide information to calculate the price for this shortlisted solution and any Additional or Associated Services you’ll need.".FormatForComparison());
        }

        public void ConfirmSolutionPrice()
        {

        }

        private int GetCatalogueItemWithPrices(CatalogueItemId id)
        {
            using var dbContext = Factory.DbContext;

            var cataloguepriceid = dbContext.CataloguePrices
                .Where(p => p.CatalogueItemId == id)
                .Select(x => x.CataloguePriceId).FirstOrDefault();

            return cataloguepriceid;
        }
    }
}
