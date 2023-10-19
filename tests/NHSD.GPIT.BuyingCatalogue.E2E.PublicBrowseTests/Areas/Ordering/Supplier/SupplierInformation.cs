using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Supplier
{
    [Collection(nameof(OrderingCollection))]
    public sealed class SupplierInformation : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const string SupplierName = "E2E Test Supplier With Contact";
        private const string SupplierNameNoPublishedSolutions = "E2E Test Supplier-No Published Solutions";
        private static readonly CallOffId CallOffId = new(90002, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public SupplierInformation(LocalWebApplicationFactory factory)
            : base(factory, typeof(SupplierController), nameof(SupplierController.Supplier), Parameters)
        {
        }

        [Fact]
        public void SupplierInformation_SelectSupplier_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Supplier information - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierObjects.SupplierAutoComplete).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_SelectSupplier_FilterSuppliers_WithMatches_ExpectedResult()
        {
            CommonActions.AutoCompleteAddValue(SupplierObjects.SupplierAutoComplete, SupplierName);
            CommonActions.WaitUntilElementIsDisplayed(SupplierObjects.SearchListBox);

            CommonActions.ElementIsDisplayed(SupplierObjects.SearchResult(0)).Should().BeTrue();
            CommonActions.ElementTextEqualTo(SupplierObjects.SearchResult(0), SupplierName).Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_SelectSupplier_FilterSuppliers_WithNoMatches_ExpectedResult()
        {
            CommonActions.ElementAddValue(SupplierObjects.SupplierAutoComplete, SupplierName + "XYZ");
            CommonActions.WaitUntilElementIsDisplayed(SupplierObjects.SearchListBox);

            CommonActions.ElementIsDisplayed(SupplierObjects.SearchResultsErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_SelectSupplier_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_SelectSupplier_NoInput_DisplaysValidationErrors()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SelectSupplier)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                SupplierObjects.SupplierAutoCompleteError,
                SelectSupplierModelValidator.SupplierSearchMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_SelectSupplier_InvalidSupplierNameSupplied_DisplaysValidationErrors()
        {
            CommonActions.AutoCompleteAddValue(SupplierObjects.SupplierAutoComplete, Strings.RandomString(50));
            CommonActions.ClickLinkElement(SupplierObjects.SearchResultsErrorMessage);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SelectSupplier)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                SupplierObjects.SupplierAutoCompleteError,
                SelectSupplierModelValidator.SupplierSearchMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_SelectSupplier_SupplierNoPublishedSolutions_DisplaysValidationErrors()
        {
            CommonActions.AutoCompleteAddValue(SupplierObjects.SupplierAutoComplete, SupplierNameNoPublishedSolutions);
            CommonActions.ClickLinkElement(SupplierObjects.SearchResultsErrorMessage);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SelectSupplier)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                SupplierObjects.SupplierAutoCompleteError,
                SelectSupplierModelValidator.SupplierSearchMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_ValidSupplierWithExistingContact_ConfirmSupplier_Expected()
        {
            CommonActions.AutoCompleteAddValue(SupplierObjects.SupplierAutoComplete, SupplierName);
            CommonActions.ClickLinkElement(SupplierObjects.SearchResult(0));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.ConfirmSupplier)).Should().BeTrue();

            CommonActions.PageTitle().Should().BeEquivalentTo($"Supplier details - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.ActionLink).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_ValidSupplierWithExistingContact_ConfirmSupplier_GoBack_Expected()
        {
            CommonActions.AutoCompleteAddValue(SupplierObjects.SupplierAutoComplete, SupplierName);
            CommonActions.ClickLinkElement(SupplierObjects.SearchResult(0));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.ConfirmSupplier)).Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SelectSupplier)).Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_ValidSupplierWithExistingContact_ConfirmSupplier_ClickActionLick_Expected()
        {
            CommonActions.AutoCompleteAddValue(SupplierObjects.SupplierAutoComplete, SupplierName);
            CommonActions.ClickLinkElement(SupplierObjects.SearchResult(0));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.ConfirmSupplier)).Should().BeTrue();

            CommonActions.ClickLinkElement(CommonSelectors.ActionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SelectSupplier)).Should().BeTrue();
        }

        [Fact]
        public async Task SupplierInformation_SelectSupplier_SupplierAlreadySelected_Expected()
        {
            await RunTestWithRetryAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var supplier = context.Suppliers.First();
                var order = context.Orders
                    .Include(o => o.Supplier)
                    .Include(o => o.SupplierContact)
                    .First(o => o.OrderNumber == CallOffId.OrderNumber && o.Revision == CallOffId.Revision);

                order.Supplier = supplier;
                await context.SaveChangesAsync();

                NavigateToUrl(
                    typeof(SupplierController),
                    nameof(SupplierController.SelectSupplier),
                    Parameters);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SupplierController),
                    nameof(SupplierController.Supplier)).Should().BeTrue();
            });
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var order = context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .First(o => o.OrderNumber == CallOffId.OrderNumber && o.Revision == CallOffId.Revision);

            order.Supplier = null;
            order.SupplierContact = null;

            context.SaveChanges();
        }
    }
}
