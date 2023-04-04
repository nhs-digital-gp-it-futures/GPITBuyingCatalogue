using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
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
    public sealed class SupplierInformationSupplierSelected : BuyerTestBase, IAsyncLifetime
    {
        private const string InternalOrgId = "CG-03F";
        private const int SupplierContactId = 2;
        private static readonly CallOffId CallOffId = new(91002, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public SupplierInformationSupplierSelected(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(SupplierController),
                nameof(SupplierController.Supplier),
                Parameters)
        {
        }

        [Fact]
        public void SupplierInformation_NavigateToSearch_Expected()
        {
            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.SelectSupplier),
                Parameters);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_NoContactSelected_Expected()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Supplier contact details - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierObjects.CreateNewContactLink).Should().BeTrue();

            CommonActions.ElementIsDisplayed(SupplierObjects.SupplierRadioContainer).Should().BeTrue();
            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(3);
            CommonActions.GetNumberOfSelectedRadioButtons().Should().Be(0);
        }

        [Fact]
        public void SupplierInformation_NoContactSelected_ClickSave_DisplaysError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                SupplierObjects.SupplierContactRadioErrorMessage,
                $"Error:{SupplierModelValidator.ContactNotSelectedErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_NoContactSelected_SelectContact_ClickSave_Expected()
        {
            CommonActions.ClickRadioButtonWithValue($"{SupplierContactId}");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void SupplierInformation_NoContactSelected_AddNewContact_NewContactSelected()
        {
            CommonActions.ClickLinkElement(SupplierObjects.CreateNewContactLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.NewContact)).Should().BeTrue();

            TextGenerators.TextInputAddText(NewContactObjects.FirstNameInput, 20);
            TextGenerators.TextInputAddText(NewContactObjects.LastNameInput, 20);
            TextGenerators.TextInputAddText(NewContactObjects.DepartmentInput, 20);
            TextGenerators.TextInputAddText(NewContactObjects.PhoneNumberInput, 20);
            TextGenerators.EmailInputAddText(NewContactObjects.EmailInput, 50);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.Supplier)).Should().BeTrue();

            CommonActions.ElementIsDisplayed(SupplierObjects.SupplierRadioContainer).Should().BeTrue();
            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(4);
            CommonActions.GetNumberOfSelectedRadioButtons().Should().Be(1);
        }

        [Fact]
        public async Task SupplierInformation_ContactSelected_Expected()
        {
            await SetSupplierContact();

            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.Supplier),
                Parameters);

            CommonActions.PageTitle().Should().BeEquivalentTo($"Supplier contact details - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierObjects.CreateNewContactLink).Should().BeTrue();

            CommonActions.ElementIsDisplayed(SupplierObjects.SupplierRadioContainer).Should().BeTrue();
            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(3);
            CommonActions.GetNumberOfSelectedRadioButtons().Should().Be(1);
        }

        [Fact]
        public async Task SupplierInformation_ContactSelected_ClickSave_Expected()
        {
            await SetSupplierContact();

            NavigateToUrl(
                typeof(SupplierController),
                nameof(SupplierController.Supplier),
                Parameters);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            using var context = GetEndToEndDbContext();

            var order = context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .First(o => o.OrderNumber == CallOffId.OrderNumber && o.Revision == CallOffId.Revision);

            order.SupplierContact = null;

            context.SaveChanges();

            return DisposeSession();
        }

        private async Task SetSupplierContact()
        {
            await using var context = GetEndToEndDbContext();

            var order = await context.Orders
                .Include(o => o.Supplier).ThenInclude(s => s.SupplierContacts)
                .Include(o => o.SupplierContact)
                .FirstAsync(o => o.OrderNumber == CallOffId.OrderNumber && o.Revision == CallOffId.Revision);

            var contact = order.Supplier.SupplierContacts.First();

            order.SupplierContact = new Contact
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Department = contact.Department,
                Email = contact.Email,
                Phone = contact.PhoneNumber,
            };

            await context.SaveChangesAsync();
        }
    }
}
