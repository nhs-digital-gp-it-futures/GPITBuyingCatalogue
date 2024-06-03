using CsvHelper;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using ProvisioningType = NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices.ProvisioningType;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionAssociatedServices
{
    public class SolutionAssociatedService : PageBase
    {
        private const decimal MaxPrice = 0.07M;
        private string service = string.Empty;

        public SolutionAssociatedService(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void AddAssociatedService(string solutionId, string priceType)
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.AssociatedServiceLink(solutionId));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.AssociatedServices))
                .Should().BeTrue();

            CommonActions.ClickLinkElement(AssociatedServicesObjects.AddAssociatedServiceLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.AddAssociatedService))
                .Should().BeTrue();

            AddAssociatedServiceDetails();
            AddListPrices(priceType, service);
        }

        public void AddAssociatedServiceDetails()
        {
            service = TextGenerators.TextInputAddText(CommonSelectors.Name, 50);
            TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);
            TextGenerators.TextInputAddText(CommonSelectors.OrderGuidance, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService))
                .Should()
                .BeTrue();
        }

        public void AddListPrices(string priceType, string service)
        {
            var serviceId = GetAssociatedServiceID();
            CommonActions.ClickLinkElement(AssociatedServicesObjects.EditPriceLink(serviceId));
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{service} - List price".FormatForComparison());

            CommonActions.ClickLinkElement(ManageListPricesObjects.AddPriceLink);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{service} - List price type".FormatForComparison());

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
                typeof(AssociatedServiceListPriceController),
                nameof(AssociatedServiceListPriceController.AddFlatListPrice))
                .Should().BeTrue();

            AddFlatPriceDetails();
            AssociatedServiceDashboard();
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
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService))
                .Should()
                .BeTrue();
        }

        public void AddTieredPrice(string priceType)
        {
            var type = priceType.Replace("_", " ");
            CommonActions.ClickRadioButtonWithText(type);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServiceListPriceController),
                nameof(AssociatedServiceListPriceController.AddTieredListPrice))
                .Should().BeTrue();

            AddTieredPriceDetails();
            AddTieredPriceTierDetails();
            AssociatedServiceDashboard();
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
                typeof(AssociatedServiceListPriceController),
                nameof(AssociatedServiceListPriceController.TieredPriceTiers))
                .Should().BeTrue();
        }

        public void AddTieredPriceTierDetails()
        {
            CommonActions.ClickLinkElement(ListPriceObjects.AddTierLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServiceListPriceController),
                nameof(AssociatedServiceListPriceController.AddTieredPriceTier))
                .Should().BeTrue();

            const decimal price = 3.14m;
            const int lowerRange = 1;

            TextGenerators.PriceInputAddPrice(AddTieredPriceTierObjects.PriceInput, price);
            CommonActions.ElementAddValue(AddTieredPriceTierObjects.LowerRangeInput, lowerRange.ToString());
            CommonActions.ClickRadioButtonWithText("Infinite upper range");

            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServiceListPriceController),
                nameof(AssociatedServiceListPriceController.TieredPriceTiers))
                .Should().BeTrue();

            CommonActions.ClickLastRadio();
            CommonActions.ClickSave();

            CommonActions.ClickSaveAndContinue();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService))
                .Should()
                .BeTrue();
        }

        public void AssociatedServiceDashboard()
        {
            CommonActions.ClickLastRadio();
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.AssociatedServices))
                .Should().BeTrue();

            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        private string GetAssociatedServiceID()
        {
            using var dbContext = Factory.DbContext;

            var serviceId = dbContext.AssociatedServices.FirstOrDefault(x => x.CatalogueItem.Name == service);
            return (serviceId != null) ? serviceId.CatalogueItemId.ToString() : string.Empty;
        }
    }
}
