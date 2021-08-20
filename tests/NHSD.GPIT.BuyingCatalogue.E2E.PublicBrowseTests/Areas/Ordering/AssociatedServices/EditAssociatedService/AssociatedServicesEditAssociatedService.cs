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
    public sealed class AssociatedServicesEditAssociatedService
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private static readonly CallOffId CallOffId = new(90008, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "-S-997");
        private static readonly string OdsCode = "03F";
        private static readonly string CatalogueItemName = "E2E Multiple Prices Associated Service";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(OdsCode), OdsCode },
                { nameof(CallOffId), CallOffId.ToString() },
                { nameof(CatalogueItemId), CatalogueItemId.ToString() },
            };

        public AssociatedServicesEditAssociatedService(LocalWebApplicationFactory factory)
        : base(
              factory,
              typeof(AssociatedServicesController),
              nameof(AssociatedServicesController.SelectAssociatedService),
              Parameters)
        {
        }

        [Fact]
        public void AssociatedServicesEditAssociatedService_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

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

            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtonItems).Should().BeFalse();
        }

        [Theory]
        [InlineData("", "Enter an agreed price")]
        [InlineData("1000", "Price cannot be greater than list price")]
        [InlineData("-1", "Price cannot be negative")]
        [InlineData("ABC", "The value 'ABC' is not valid for AgreedPrice.")]
        public void AssociatedServicesEditAssociatedService_AgreedPrice_IncorrectInput_ThrowsErrors(string errorValue, string expectedErrorMessage)
        {
            CommonActions.ClearInputElement(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionAgreedPriceInput);

            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionAgreedPriceInput,
                errorValue);

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
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionAgreedPriceInputErrorMessage,
                expectedErrorMessage)
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData("", "Enter a quantity")]
        [InlineData("0", "Quantity must be greater than 0")]
        [InlineData("-1", "Quantity must be greater than 0")]
        [InlineData("ABC", "The value 'ABC' is not valid for Quantity.")]
        public void AssociatedServicesEditAssociatedService_Quantity_IncorrectInput_ThrowsErrors(
            string errorValue,
            string expectedErrorMessage)
        {
            CommonActions.ClearInputElement(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstQuantityInput);

            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstQuantityInput,
                errorValue);

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
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstQuantityInputErrorMessage,
                expectedErrorMessage)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AssociatedServicesEditAssociatedService_ClickDeleteAssociatedService_ExpectedResult()
        {
            CommonActions.ClickLinkElement(
                Objects.Ordering.AssociatedServices.AssociatedServicesEditAssociatedServiceDeleteAssociatedServiceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeleteAssociatedServiceController),
                nameof(DeleteAssociatedServiceController.DeleteAssociatedService))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AssociatedServicesEditAssociatedService_CorrectInput_ExpectedResults()
        {
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
                .SingleOrDefault(cp => cp.CataloguePriceId == 10);

            var model = new CreateOrderItemModel
            {
                CallOffId = CallOffId,
                CatalogueItemId = CatalogueItemId,
                CommencementDate = DateTime.UtcNow.AddDays(1),
                CatalogueItemName = CatalogueItemName,
                CataloguePrice = price,
                Quantity = 123,
                EstimationPeriod = EntityFramework.Catalogue.Models.TimeUnit.PerMonth,
                IsNewSolution = true,
                CurrencySymbol = "£",
                AgreedPrice = 100,
                CatalogueItemType = EntityFramework.Catalogue.Models.CatalogueItemType.AssociatedService,
                ServiceRecipients = new List<OrderItemRecipientModel>
                {
                    new()
                    {
                        OdsCode = OdsCode,
                        Selected = true,
                        Name = "Hull CCG",
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
