using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.AdditionalServices
{
    public sealed class AdditionalServicesEditAdditionalServices
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        public static readonly object[][] DateIncorrectInputData =
            new object[][]
            {
                new object[]
                {
                    DateTime.UtcNow.AddDays(-1),
                    "Error: Planned delivery date must be in the future",
                },
                new object[]
                {
                    DateTime.UtcNow.AddYears(10),
                    "Error: Planned delivery date must be within 42 months from the commencement date for this Call - off Agreement",
                },
            };

        private static readonly CallOffId CallOffId = new(90007, 1);
        private static readonly string OdsCode = "03F";
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A999");
        private static readonly string CatalogueItemName = "E2E Multiple Prices Additional Service";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(OdsCode), OdsCode },
                { nameof(CallOffId), CallOffId.ToString() },
                { nameof(CatalogueItemId), CatalogueItemId.ToString() },
            };

        public AdditionalServicesEditAdditionalServices(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.SelectAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public void AdditionalServicesEditAddtionalService_AllSectionsDisplayed()
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
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstDateDayInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstDateMonthInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstDateYearInput)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionEditServiceRecipientsButton)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.AdditionalServices.AdditionalServicesEditAdditionalServiceDeleteAdditionalServiceLink)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesEditAdditionalService_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceRecipientsDateController),
                nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate))
            .Should()
            .BeTrue();
        }

        [Theory]
        [InlineData("", "Enter an agreed price")]
        [InlineData("1000", "Price cannot be greater than list price")]
        [InlineData("-1", "Price cannot be negative")]
        [InlineData("ABC", "The value 'ABC' is not valid for AgreedPrice.")]
        public void AdditionalServicesEditAddtionalService_AgreedPrice_IncorrectInput_ThrowsErrors(
            string errorValue,
            string expectedErrorMessage)
        {
            CommonActions.ClearInputElement(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionAgreedPriceInput);

            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionAgreedPriceInput,
                errorValue);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.EditAdditionalService))
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
        public void AdditionalServicesEditAddtionalService_Quantity_IncorrectInput_ThrowsErrors(
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
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.EditAdditionalService))
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

        [Theory]
        [MemberData(nameof(DateIncorrectInputData))]
        public void AdditionalServicesEditAddtionalService_Date_IncorrectInput_ThrowsError(
            DateTime errorValue,
            string expectedErrorMessage)
        {
            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstDateDayInput,
                errorValue.Day.ToString("00"));

            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstDateMonthInput,
                errorValue.Month.ToString("00"));

            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstDateYearInput,
                errorValue.Year.ToString("0000"));

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.AdditionalServices.AddtionalServiceEditAdditionalServiceFirstDateInputErrorMessage,
                expectedErrorMessage)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesEditAddtionalService_Date_NoInput_ThrowsError()
        {
            CommonActions.ClearInputElement(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstDateDayInput);

            CommonActions.ClearInputElement(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstDateMonthInput);

            CommonActions.ClearInputElement(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstDateYearInput);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.AdditionalServices.AddtionalServiceEditAdditionalServiceFirstDateInputErrorMessage,
                "Error: Planned delivery date must be a real date")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesEditAddtionalService_ClickEditServiceRecipients_ExpectedResult()
        {
            CommonActions.ClickLinkElement(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionEditServiceRecipientsButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceRecipientsController),
                nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesEditAddtionalService_ClickDeleteAdditionalService_ExpectedResult()
        {
            CommonActions.ClickLinkElement(
                Objects.Ordering.AdditionalServices.AdditionalServicesEditAdditionalServiceDeleteAdditionalServiceLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeleteAdditionalServiceController),
                nameof(DeleteAdditionalServiceController.DeleteAdditionalService))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesEditAddtionalService_CorrectInput_ExpectedResults()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.Index))
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(By.LinkText(CatalogueItemName)).Should().BeTrue();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            InitializeMemoryCacheHander(OdsCode);

            using var context = GetEndToEndDbContext();
            var price = context
                .CataloguePrices
                .Include(cp => cp.PricingUnit)
                .SingleOrDefault(cp => cp.CataloguePriceId == 6);

            var firstServiceRecipient = MemoryCache.GetServiceRecipients().FirstOrDefault();

            var model = new CreateOrderItemModel
            {
                CallOffId = CallOffId,
                CatalogueItemId = CatalogueItemId,
                CommencementDate = DateTime.UtcNow.AddDays(1),
                CatalogueItemName = CatalogueItemName,
                CataloguePrice = price,
                ServiceRecipients = new()
                {
                    new()
                    {
                        Name = firstServiceRecipient.Name,
                        OdsCode = firstServiceRecipient.OrgId,
                        Selected = true,
                        Quantity = 123,
                        DeliveryDate = DateTime.UtcNow.AddDays(2),
                    },
                },
                Quantity = 123,
                EstimationPeriod = EntityFramework.Catalogue.Models.TimeUnit.PerMonth,
                IsNewSolution = true,
                CurrencySymbol = "£",
                AgreedPrice = 100,
            };

            Session.SetOrderStateToSessionAsync(model);

            NavigateToUrl(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
