using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class SupplierInformation
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string OdsCode = "03F";
        private const string SearchWithContact = "E2E Test Supplier With Contact";
        private const string SearchNoContact = "E2E Test Supplier";
        private static readonly CallOffId CallOffId = new(90002, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OdsCode), OdsCode },
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
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Find supplier information for {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierSearchInput).Should().BeTrue();
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

            CommonActions.PageTitle().Should().BeEquivalentTo("Suppliers found".FormatForComparison());

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

            CommonActions.PageTitle().Should().BeEquivalentTo("Suppliers found".FormatForComparison());

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

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.SupplierInformation.SupplierRadioErrorMessage,
                "Error: Please select a supplier").Should().BeTrue();
        }

        [Fact]
        public async Task SupplierInformation_ValidSupplierWithExistingContact_SelectSupplier_Expected()
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
                nameof(SupplierController.Supplier)).Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var order = await context.Orders
                .Include(o => o.Supplier).ThenInclude(s => s.SupplierContacts)
                .Include(o => o.SupplierContact)
                .SingleAsync(o => o.Id == CallOffId.Id);

            CommonActions.PageTitle().Should().BeEquivalentTo($"Supplier information for {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierAddressLine1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierFirstName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierLastName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierEmail).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierPhone).Should().BeTrue();

            CommonActions.ElementTextEqualToo(Objects.Ordering.SupplierInformation.SupplierName, order.Supplier.Name).Should().BeTrue();
            CommonActions.ElementTextEqualToo(Objects.Ordering.SupplierInformation.SupplierAddressLine1, order.Supplier.Address.Line1).Should().BeTrue();
            CommonActions.InputValueEqualToo(Objects.Ordering.SupplierInformation.SupplierFirstName, order.Supplier.SupplierContacts.First().FirstName).Should().BeTrue();
            CommonActions.InputValueEqualToo(Objects.Ordering.SupplierInformation.SupplierLastName, order.Supplier.SupplierContacts.First().LastName).Should().BeTrue();
            CommonActions.InputValueEqualToo(Objects.Ordering.SupplierInformation.SupplierEmail, order.Supplier.SupplierContacts.First().Email).Should().BeTrue();
            CommonActions.InputValueEqualToo(Objects.Ordering.SupplierInformation.SupplierPhone, order.Supplier.SupplierContacts.First().PhoneNumber).Should().BeTrue();
        }

        [Fact]
        public async Task SupplierInformation_ValidSupplierWithoutExistingContact_Expected()
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "search", SearchNoContact },
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
                nameof(SupplierController.Supplier)).Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var order = await context.Orders
                .Include(o => o.Supplier).ThenInclude(s => s.SupplierContacts)
                .Include(o => o.SupplierContact)
                .SingleAsync(o => o.Id == CallOffId.Id);

            CommonActions.PageTitle().Should().BeEquivalentTo($"Supplier information for {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierAddressLine1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierFirstName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierLastName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierEmail).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.SupplierInformation.SupplierPhone).Should().BeTrue();

            CommonActions.ElementTextEqualToo(Objects.Ordering.SupplierInformation.SupplierName, order.Supplier.Name).Should().BeTrue();
            CommonActions.ElementTextEqualToo(Objects.Ordering.SupplierInformation.SupplierAddressLine1, order.Supplier.Address.Line1).Should().BeTrue();
            CommonActions.InputElementIsEmpty(Objects.Ordering.SupplierInformation.SupplierFirstName).Should().BeTrue();
            CommonActions.InputElementIsEmpty(Objects.Ordering.SupplierInformation.SupplierLastName).Should().BeTrue();
            CommonActions.InputElementIsEmpty(Objects.Ordering.SupplierInformation.SupplierEmail).Should().BeTrue();
            CommonActions.InputElementIsEmpty(Objects.Ordering.SupplierInformation.SupplierPhone).Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_SupplierWithoutContact_SelectSupplier_LeaveContactEmpty_ThrowsError()
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "search", SearchNoContact },
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
                nameof(SupplierController.Supplier)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage("PrimaryContact.FirstName", "First Name Required").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage("PrimaryContact.LastName", "Last Name Required").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage("PrimaryContact.Email", "Email Address Required").Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage("PrimaryContact.Phone", "Telephone Number Required").Should().BeTrue();
        }

        [Fact]
        public async Task SupplierInformation_SupplierWithoutContact_FillInputBoxes_Expected()
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "search", SearchNoContact },
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
                nameof(SupplierController.Supplier)).Should().BeTrue();

            var firstName = TextGenerators.TextInputAddText(Objects.Ordering.SupplierInformation.SupplierFirstName, 100);
            var lastName = TextGenerators.TextInputAddText(Objects.Ordering.SupplierInformation.SupplierLastName, 100);
            var email = TextGenerators.EmailInputAddText(Objects.Ordering.SupplierInformation.SupplierEmail, 256);
            var phone = TextGenerators.TextInputAddText(Objects.Ordering.SupplierInformation.SupplierPhone, 35);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var order = await context.Orders.Include(o => o.SupplierContact)
                .SingleAsync(o => o.Id == CallOffId.Id);

            order.SupplierContact.FirstName.Should().BeEquivalentTo(firstName);
            order.SupplierContact.LastName.Should().BeEquivalentTo(lastName);
            order.SupplierContact.Email.Should().BeEquivalentTo(email);
            order.SupplierContact.Phone.Should().BeEquivalentTo(phone);
        }

        [Fact]
        public void SupplierInformation_SupplierWithoutContact_InputNotAnEmail_EmailFieldThrowsError()
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "search", SearchNoContact },
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
                nameof(SupplierController.Supplier)).Should().BeTrue();

            TextGenerators.TextInputAddText(Objects.Ordering.SupplierInformation.SupplierFirstName, 100);
            TextGenerators.TextInputAddText(Objects.Ordering.SupplierInformation.SupplierLastName, 100);
            TextGenerators.TextInputAddText(Objects.Ordering.SupplierInformation.SupplierEmail, 256);
            TextGenerators.TextInputAddText(Objects.Ordering.SupplierInformation.SupplierPhone, 35);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage("PrimaryContact.Email", "The Email field is not a valid e-mail address.").Should().BeTrue();
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
