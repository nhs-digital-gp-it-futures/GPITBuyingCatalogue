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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Supplier
{
    public sealed class SupplierInformation
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "03F";
        private const string SearchWithContact = "E2E Test Supplier With Contact";
        private const string SearchNoContact = "E2E Test Supplier";
        private static readonly CallOffId CallOffId = new(90002, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public SupplierInformation(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SupplierController),
                  nameof(SupplierController.Supplier),
                  Parameters)
        {
        }

        [Fact]
        public void SupplierInformation_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Find supplier information - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierSearchInput).Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void SupplierInformation_NoInput_ReturnsNoSupplierFound()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SupplierSearchSelect)).Should().BeTrue();

            CommonActions.PageTitle().Should().BeEquivalentTo("No supplier found".FormatForComparison());
        }

        [Fact]
        public void SupplierInformation_InvalidSupplierNameSupplied_ReturnsNoSupplierFound()
        {
            TextGenerators.TextInputAddText(Objects.Ordering.SupplierInformation.SupplierSearchInput, 500);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SupplierSearchSelect)).Should().BeTrue();

            CommonActions.PageTitle().Should().BeEquivalentTo("No supplier found".FormatForComparison());
        }

        [Fact]
        public void SupplierInformation_ValidSupplierNameSupplied_ReturnsMultipleSuppliers()
        {
            // Supplier name set in BuyingCatalogueSeedData
            CommonActions.ElementAddValue(Objects.Ordering.SupplierInformation.SupplierSearchInput, "E2E Test Supplier");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SupplierController),
                    nameof(SupplierController.SupplierSearchSelect)).Should().BeTrue();

            CommonActions.PageTitle().Should().BeEquivalentTo($"Suppliers found - Order {CallOffId}".FormatForComparison());

            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierRadioContainer);

            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(2);
        }

        [Fact]
        public void SupplierInformation_ValidSupplierNameSupplied_ReturnsSingleSupplier()
        {
            // Supplier name set in BuyingCatalogueSeedData
            CommonActions.ElementAddValue(Objects.Ordering.SupplierInformation.SupplierSearchInput, "E2E Test Supplier With Contact");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SupplierController),
                    nameof(SupplierController.SupplierSearchSelect)).Should().BeTrue();

            CommonActions.PageTitle().Should().BeEquivalentTo($"Suppliers found - Order {CallOffId}".FormatForComparison());

            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierRadioContainer);

            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(1);
        }

        [Fact]
        public void SupplierInformation_ValidSupplierWithExistingContact_DontSelectSupplier_ThrowsError()
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "search", SearchWithContact },
            };

            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.SupplierSearchSelect),
                Parameters,
                queryParameters);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SupplierSearchSelect)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.SupplierInformation.SupplierRadioErrorMessage,
                "Error: Please select a supplier").Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_ValidSupplierWithExistingContact_ConfirmSupplier_Expected()
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "search", SearchWithContact },
            };

            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.SupplierSearchSelect),
                Parameters,
                queryParameters);

            CommonActions.ClickRadioButtonWithText(queryParameters["search"]);

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
            var queryParameters = new Dictionary<string, string>
            {
                { "search", SearchWithContact },
            };

            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.SupplierSearchSelect),
                Parameters,
                queryParameters);

            CommonActions.ClickRadioButtonWithText(queryParameters["search"]);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.ConfirmSupplier)).Should().BeTrue();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SupplierSearchSelect)).Should().BeTrue();

            CommonActions.GetNumberOfSelectedRadioButtons().Should().Be(1);
        }

        [Fact]
        public void SupplierInformation_ValidSupplierWithExistingContact_ConfirmSupplier_ClickActionLick_Expected()
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "search", SearchWithContact },
            };

            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.SupplierSearchSelect),
                Parameters,
                queryParameters);

            CommonActions.ClickRadioButtonWithText(queryParameters["search"]);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.ConfirmSupplier)).Should().BeTrue();

            CommonActions.ClickLinkElement(CommonSelectors.ActionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SupplierSearchSelect)).Should().BeTrue();

            CommonActions.GetNumberOfSelectedRadioButtons().Should().Be(1);
        }

        [Fact]
        public async Task SupplierInformation_SupplierSearch_SupplierAlreadySelected_Expected()
        {
            await using var context = GetEndToEndDbContext();
            var supplier = context.Suppliers.First();
            var order = context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Single(o => o.Id == CallOffId.Id);

            order.Supplier = supplier;
            await context.SaveChangesAsync();

            var queryParameters = new Dictionary<string, string>
            {
                { "search", SearchWithContact },
            };

            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.SupplierSearchSelect),
                Parameters,
                queryParameters);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var order = context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Single(o => o.Id == CallOffId.Id);

            order.Supplier = null;
            order.SupplierContact = null;

            context.SaveChanges();
        }
    }
}
