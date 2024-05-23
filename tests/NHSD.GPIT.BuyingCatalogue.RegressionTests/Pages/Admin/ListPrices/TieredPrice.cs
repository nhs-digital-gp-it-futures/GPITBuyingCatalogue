using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ListPrices
{
    public class TieredPrice : PageBase
    {
        public TieredPrice(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddTieredPrice(string priceType)
        {
            string type = priceType.Replace("_", " ");
            CommonActions.ClickRadioButtonWithText(type);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.AddTieredListPrice))
                .Should().BeTrue();

            AddTieredPriceDetails();
            AddTieredPriceTierDetails();
            ManageListPrice();
        }

        public void AddTieredPriceDetails()
        {
            CommonActions.ClickRadioButtonWithText(ProvisioningType.Per_patient_per_year.ToString().Replace("_", " "));
            CommonActions.ClickRadioButtonWithText(CalculationType.Volume.ToString());

            TextGenerators.TextInputAddText(ListPriceObjects.UnitDescriptionInput, 100);
            TextGenerators.TextInputAddText(ListPriceObjects.RangeDefinitionInput, 100);
            TextGenerators.TextInputAddText(ListPriceObjects.UnitDefinitionInput, 500);

            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.TieredPriceTiers))
                .Should().BeTrue();
        }

        public void AddTieredPriceTierDetails()
        {
            CommonActions.ClickLinkElement(ListPriceObjects.AddTierLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.AddTieredPriceTier))
                .Should().BeTrue();

            const decimal price = 3.14m;
            const int lowerRange = 1;

            TextGenerators.PriceInputAddPrice(AddTieredPriceTierObjects.PriceInput, price);
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.LowerRangeInput, lowerRange.ToString());
            CommonActions.ClickRadioButtonWithText("Infinite upper range");

            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.TieredPriceTiers))
                .Should().BeTrue();
        }

        public void ManageListPrice()
        {
            CommonActions.ClickRadioButtonWithText("Publish");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionListPriceController),
                nameof(CatalogueSolutionListPriceController.Index))
                .Should().BeTrue();
        }
    }
}
