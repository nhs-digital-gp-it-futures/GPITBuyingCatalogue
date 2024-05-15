using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ListPrices
{
    public class FlatPrice : PageBase
    {
        private const decimal MaxPrice = 0.07M;

        public FlatPrice(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddFlatPrice(string priceType)
        {
            string type = priceType.Replace("_", " ");
            CommonActions.ClickRadioButtonWithText(type);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.AddFlatListPrice))
                .Should().BeTrue();

            AddFlatPriceDetails();
        }

        public void AddFlatPriceDetails()
        {
            CommonActions.ClickRadioButtonWithText(ProvisioningType.Per_patient_per_year.ToString().Replace("_", " "));
            CommonActions.ClickRadioButtonWithText(CalculationType.Single_fixed.ToString().Replace("_", " "));

            TextGenerators.PriceInputAddPrice(ListPriceObjects.PriceInput, MaxPrice);
            TextGenerators.TextInputAddText(ListPriceObjects.UnitDescriptionInput, 100);
            TextGenerators.TextInputAddText(ListPriceObjects.RangeDefinitionInput, 30);
            TextGenerators.TextInputAddText(ListPriceObjects.UnitDefinitionInput, 500);
            CommonActions.ClickLastRadio();

            CommonActions.ClickSave();
        }
    }
}
