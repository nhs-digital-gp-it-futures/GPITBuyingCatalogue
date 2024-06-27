using FluentAssertions;
using Microsoft.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionAssociatedServices
{
    public class SolutionAssociatedService : PageBase
    {
        private const decimal MaxPrice = 0.07M;
        private string service = string.Empty;

        public SolutionAssociatedService(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            ListPricesForServices = new ListPricesForServices(driver, commonActions);
            Factory = factory;
        }

        public ListPricesForServices ListPricesForServices { get; }

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

        private void AddAssociatedServiceDetails()
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

        private void AddListPrices(string priceType, string service)
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
                ListPricesForServices.AddFlatPrice(priceType, service);
                AssociatedServiceDashboard();
            }
            else
            {
                ListPricesForServices.AddTieredPrice(priceType, service);
                AssociatedServiceDashboard();
            }
        }

        private void AssociatedServiceDashboard()
        {
            CommonActions.ClickLastRadio();
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.AssociatedServices))
                .Should().BeTrue();

            CommonActions.ClickAllCheckboxes();
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
