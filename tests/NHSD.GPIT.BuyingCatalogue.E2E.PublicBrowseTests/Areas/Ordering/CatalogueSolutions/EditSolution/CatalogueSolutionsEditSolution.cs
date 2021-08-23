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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutionsEditSolution
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

        private const string OdsCode = "03F";
        private const string CatalogueItemName = "E2E With Contact Multiple Prices";
        private static readonly CallOffId CallOffId = new(90004, 01);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(OdsCode), OdsCode },
                { nameof(CallOffId), CallOffId.ToString() },
                { nameof(CatalogueItemId), CatalogueItemId.ToString() },
            };

        public CatalogueSolutionsEditSolution(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.SelectSolution),
                  Parameters)
        {
        }

        [Fact]
        public void CatalogueSolutionsEditSolution_AllSectionsDisplayed()
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
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionDeleteSolutionLink)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsEditSolution_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionRecipientsDateController),
                nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate))
            .Should()
            .BeTrue();
        }

        [Theory]
        [InlineData("", "Enter an agreed price")]
        [InlineData("1000", "Price cannot be greater than list price")]
        [InlineData("-1", "Price cannot be negative")]
        [InlineData("ABC", "The value 'ABC' is not valid for AgreedPrice.")]
        public void CatalogueSolutionsEditSolution_AgreedPrice_IncorrectInput_ThrowsErrors(string errorValue, string expectedErrorMessage)
        {
            CommonActions.ClearInputElement(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionAgreedPriceInput);

            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionAgreedPriceInput,
                errorValue);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.EditSolution))
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
        public void CatalogueSolutionsEditSolution_Quantity_IncorrectInput_ThrowsErrors(string errorValue, string expectedErrorMessage)
        {
            CommonActions.ClearInputElement(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstQuantityInput);

            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstQuantityInput,
                errorValue);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.EditSolution))
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
        public void CatalogueSolutionsEditSolution_Date_IncorrectInput_ThrowsError(DateTime errorValue, string expectedErrorMessage)
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
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.EditSolution))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstDateInputErrorMessage,
                expectedErrorMessage)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsEditSolution_Date_NoInput_ThrowsError()
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
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.EditSolution))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionFirstDateInputErrorMessage,
                "Error: Planned delivery date must be a real date")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsEditSolution_ClickEditServiceRecipients_ExpectedResult()
        {
            CommonActions.ClickLinkElement(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionEditServiceRecipientsButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionRecipientsController),
                nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsEditSolution_ClickDeleteCatalogueSolution_ExpectedResult()
        {
            CommonActions.ClickLinkElement(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsEditSolutionDeleteSolutionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeleteCatalogueSolutionController),
                nameof(DeleteCatalogueSolutionController.DeleteSolution))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsEditSolution_CorrectInput_ExpectedResults()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.Index))
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
                .SingleOrDefault(cp => cp.CataloguePriceId == 1);

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
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditSolution),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
