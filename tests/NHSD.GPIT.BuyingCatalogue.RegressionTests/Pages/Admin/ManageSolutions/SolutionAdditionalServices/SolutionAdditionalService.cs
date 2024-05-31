using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using ProvisioningType = NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices.ProvisioningType;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.AdditionalService
{
    public class SolutionAdditionalService : PageBase
    {
        private const decimal MaxPrice = 0.07M;
        private string service = string.Empty;

        public SolutionAdditionalService(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            FlatPrice = new FlatPrice(driver, commonActions);
            TieredPrice = new TieredPrice(driver, commonActions);
            Factory = factory;
        }

        public FlatPrice FlatPrice { get; }

        public TieredPrice TieredPrice { get; }

        public LocalWebApplicationFactory Factory { get; }

        public void AddAdditionlService(string solutionId, string priceType)
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.AdditionalServiceLink(solutionId));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.Index))
                .Should().BeTrue();

            CommonActions.ClickLinkElement(AdditionalServicesObjects.AddAdditionalServiceLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.AddAdditionalService))
                .Should().BeTrue();

            AddAdditionalServiceDetails();
            AddCapbilities();
            AddListPrices(priceType);
        }

        public void AddAdditionalServiceDetails()
        {
            service = TextGenerators.TextInputAddText(CommonSelectors.Name, 50);
            TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();
        }

        public void AddCapbilities()
        {
            var serviceId = GetAdditionalServiceID();
            CommonActions.ClickLinkElement(AdditionalServicesObjects.EditCapabilitiesLink(serviceId));

            using var dbContext = Factory.DbContext;
            var capabilities = dbContext.Capabilities.Include(c => c.Epics).Where(x => x.Status == CapabilityStatus.Effective).ToList().Take(5);

            foreach (var capability in capabilities)
                CommonActions.ClickCheckboxByLabel($"({capability.CapabilityRef}) {capability.Name}");

            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();
        }

        public void AddListPrices(string priceType)
        {
            var serviceId = GetAdditionalServiceID();
            CommonActions.ClickLinkElement(AdditionalServicesObjects.EditPriceLink(serviceId));

            CommonActions.ClickLinkElement(ManageListPricesObjects.AddPriceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.ListPriceType))
                .Should()
                .BeTrue();
            if (priceType == ListPriceTypes.Flat_price.ToString())
            {
                AddFlatPrice(priceType);
            }
            else
            {
              AddTieredPrice(priceType);
            }
        }

        public void AddFlatPrice(string priceType)
        {
            var type = priceType.Replace("_", " ");
            CommonActions.ClickRadioButtonWithText(type);
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.AddFlatListPrice))
                .Should().BeTrue();

            AddFlatPriceDetails();
            AdditionalServiceDashboard();
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
            CommonActions.ClickSaveAndContinue();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();
        }

        public void AddTieredPrice(string priceType)
        {
            var type = priceType.Replace("_", " ");
            CommonActions.ClickRadioButtonWithText(type);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.AddTieredListPrice))
                .Should().BeTrue();

            AddTieredPriceDetails();
            AddTieredPriceTierDetails();
            AdditionalServiceDashboard();
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
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers))
                .Should().BeTrue();
        }

        public void AddTieredPriceTierDetails()
        {
            CommonActions.ClickLinkElement(ListPriceObjects.AddTierLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.AddTieredPriceTier))
                .Should().BeTrue();

            const decimal price = 3.14m;
            const int lowerRange = 1;

            TextGenerators.PriceInputAddPrice(AddTieredPriceTierObjects.PriceInput, price);
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.LowerRangeInput, lowerRange.ToString());
            CommonActions.ClickRadioButtonWithText("Infinite upper range");

            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceListPriceController),
                nameof(AdditionalServiceListPriceController.TieredPriceTiers))
                .Should().BeTrue();

            CommonActions.ClickLastRadio();
            CommonActions.ClickSave();

            CommonActions.ClickSaveAndContinue();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();
        }

        public void AdditionalServiceDashboard()
        {
            CommonActions.ClickLastRadio();
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.Index))
                .Should().BeTrue();

            CommonActions.ClickSaveAndContinue();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        private string GetAdditionalServiceID()
        {
            using var dbContext = Factory.DbContext;

            var serviceId = dbContext.AdditionalServices.FirstOrDefault(x => x.CatalogueItem.Name == service);
            return (serviceId != null) ? serviceId.CatalogueItemId.ToString() : string.Empty;
        }
    }
}
