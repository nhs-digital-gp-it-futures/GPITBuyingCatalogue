using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.AssociatedServices
{
    public sealed class AssociatedServicesEditAssociatedServiceOnDemandPrice
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string OdsCode = "03F";
        private const string CatalogueItemName = "E2E Multiple Prices Associated Service";
        private static readonly CallOffId CallOffId = new(90008, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "-S-997");

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(OdsCode), OdsCode },
                { nameof(CallOffId), CallOffId.ToString() },
                { nameof(CatalogueItemId), CatalogueItemId.ToString() },
            };

        public AssociatedServicesEditAssociatedServiceOnDemandPrice(LocalWebApplicationFactory factory)
        : base(
              factory,
              typeof(AssociatedServicesController),
              nameof(AssociatedServicesController.SelectAssociatedService),
              Parameters)
        {
        }

        [Fact]
        public void AssociatedServicesEditAssociatedServiceOnDemandPrice_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionAgreedPriceInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstQuantityInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.AssociatedServices.AssociatedServicesEditAssociatedServiceDeleteAssociatedServiceLink)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtonItems).Should().BeTrue();
        }

        [Fact]
        public void AssociatedServicesEditAssociatedServiceOnDemandPrice_DontSelectTimeUnit_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService))
            .Should()
            .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.AssociatedServices.AssociatedServiceEditAssociatedServiceEstimationPeriodErrorMessage,
                "Error: Time Unit is required")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AssociatedServicesEditAssociatedServiceOnDemandPrice_CorrectInput_ExpectedResults()
        {
            CommonActions.ClickRadioButtonWithValue("PerMonth");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.Index))
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(By.LinkText(CatalogueItemName)).Should().BeTrue();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            using var context = GetEndToEndDbContext();
            var price = context
                .CataloguePrices
                .Include(cp => cp.PricingUnit)
                .SingleOrDefault(cp => cp.CataloguePriceId == 11);

            var model = new CreateOrderItemModel
            {
                CallOffId = CallOffId,
                CatalogueItemId = CatalogueItemId,
                CommencementDate = DateTime.UtcNow.AddDays(1),
                CatalogueItemName = CatalogueItemName,
                CataloguePrice = price,
                Quantity = 123,
                IsNewSolution = true,
                CurrencySymbol = "£",
                AgreedPrice = 100,
                CatalogueItemType = EntityFramework.Catalogue.Models.CatalogueItemType.AssociatedService,
                ServiceRecipients = new List<OrderItemRecipientModel>
                {
                    new()
                    {
                        Name = "Hull CCG",
                        OdsCode = OdsCode,
                        Selected = true,
                        Quantity = 123,
                        DeliveryDate = DateTime.UtcNow.AddDays(2),
                    },
                },
            };

            Session.SetOrderStateToSessionAsync(model);

            NavigateToUrl(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
