using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;
using ProvisioningType = NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices.ProvisioningType;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions
{
    public class ListPricesForServices : PageBase
    {
        private const decimal MaxPrice = 0.07M;
        private string service = string.Empty;

        public ListPricesForServices(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddFlatPrice(string priceType, string service)
        {
            var type = priceType.Replace("_", " ");
            CommonActions.ClickRadioButtonWithText(type);
            CommonActions.ClickSave();
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{service} - Add a flat list price".FormatForComparison());

            AddFlatPriceDetails(service);
        }

        public void AddFlatPriceDetails(string service)
        {
            CommonActions.ClickRadioButtonWithText(ProvisioningType.Per_patient_per_year.ToString().Replace("_", " "));
            CommonActions.ClickRadioButtonWithText(CalculationType.Single_fixed.ToString().Replace("_", " "));

            TextGenerators.PriceInputAddPrice(ListPriceObjects.PriceInput, MaxPrice);
            TextGenerators.TextInputAddText(ListPriceObjects.UnitDescriptionInput, 100);
            TextGenerators.TextInputAddText(ListPriceObjects.RangeDefinitionInput, 30);
            TextGenerators.TextInputAddText(ListPriceObjects.UnitDefinitionInput, 500);
            CommonActions.ClickLastRadio();

            CommonActions.ClickSave();
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{service} - List price".FormatForComparison());

            CommonActions.ClickSaveAndContinue();
        }

        public void AddTieredPrice(string priceType, string service)
        {
            var type = priceType.Replace("_", " ");
            CommonActions.ClickRadioButtonWithText(type);
            CommonActions.ClickSave();
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{service} - Add a tiered list price".FormatForComparison());

            AddTieredPriceDetails(service);
            AddTieredPriceTierDetails(service);
        }

        public void AddTieredPriceDetails(string service)
        {
            CommonActions.ClickRadioButtonWithText(ProvisioningType.Per_patient_per_year.ToString().Replace("_", " "));
            CommonActions.ClickRadioButtonWithText(CalculationType.Volume.ToString());

            TextGenerators.TextInputAddText(ListPriceObjects.UnitDescriptionInput, 100);
            TextGenerators.TextInputAddText(ListPriceObjects.RangeDefinitionInput, 100);
            TextGenerators.TextInputAddText(ListPriceObjects.UnitDefinitionInput, 500);

            CommonActions.ClickSave();
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{service} - Tiered list price information".FormatForComparison());
        }

        public void AddTieredPriceTierDetails(string service)
        {
            CommonActions.ClickLinkElement(ListPriceObjects.AddTierLink);
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{service} - Add a pricing tier".FormatForComparison());

            const decimal price = 3.14m;
            const int lowerRange = 1;

            TextGenerators.PriceInputAddPrice(AddTieredPriceTierObjects.PriceInput, price);
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.LowerRangeInput, lowerRange.ToString());
            CommonActions.ClickRadioButtonWithText("Infinite upper range");

            CommonActions.ClickSave();
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{service} - Tiered list price information".FormatForComparison());

            CommonActions.ClickLastRadio();
            CommonActions.ClickSave();
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{service} - List price".FormatForComparison());

            CommonActions.ClickSaveAndContinue();
        }
    }
}
