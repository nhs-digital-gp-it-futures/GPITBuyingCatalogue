using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.AdditionalService
{
    public class SolutionAdditionalService : PageBase
    {
        private const decimal MaxPrice = 0.07M;
        private string service = string.Empty;

        public SolutionAdditionalService(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            ListPricesForServices = new ListPricesForServices(driver, commonActions);
            Factory = factory;
        }

        public ListPricesForServices ListPricesForServices { get; }

        public LocalWebApplicationFactory Factory { get; }

        public void AddAdditionalService(string solutionId, string priceType)
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
            AddCapabilities();
            AddListPrices(priceType, service);
        }

        private void AddAdditionalServiceDetails()
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

        private void AddCapabilities()
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

        private void AddListPrices(string priceType, string service)
        {
            var serviceId = GetAdditionalServiceID();
            CommonActions.ClickLinkElement(AdditionalServicesObjects.EditPriceLink(serviceId));
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{service} - List price".FormatForComparison());

            CommonActions.ClickLinkElement(ManageListPricesObjects.AddPriceLink);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{service} - List price type".FormatForComparison());

            if (priceType == ListPriceTypes.Flat_price.ToString())
            {
                ListPricesForServices.AddFlatPrice(priceType, service);
                AdditionalServiceDashboard();
            }
            else
            {
                ListPricesForServices.AddTieredPrice(priceType, service);
                AdditionalServiceDashboard();
            }
        }

        private void AdditionalServiceDashboard()
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
