using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageSuppliers;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditSupplierContact : AuthorityTestBase
    {
        private const int SupplierId = 99998;
        private const int ContactId = 3;
        private const int ReferencedContactId = 2;
        private static readonly CatalogueItemId ReferencingSolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SupplierId), SupplierId.ToString() },
            { nameof(ContactId), ContactId.ToString() },
        };

        private static readonly Dictionary<string, string> ReferencedContactParameters = new()
        {
            { nameof(SupplierId), SupplierId.ToString() },
            { nameof(ContactId), ReferencedContactId.ToString() },
        };

        public EditSupplierContact(LocalWebApplicationFactory factory)
            : base(factory, typeof(SuppliersController), nameof(SuppliersController.EditSupplierContact), Parameters)
        {
        }

        [Fact]
        public void EditSupplierContact_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierContactObjects.FirstNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierContactObjects.LastNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierContactObjects.DepartmentInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierContactObjects.PhoneNumberInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierContactObjects.EmailInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierContactObjects.ReferencingSolutionsTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(SupplierContactObjects.NoReferencingSolutionsInset).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierContactObjects.DeleteLink).Should().BeTrue();
        }

        [Fact]
        public async Task EditSupplierContact_ReferencedContact_SectionsDisplayed()
        {
            await AddSupplierContactToSolutionIfNotExists();

            NavigateToUrl(
                typeof(SuppliersController),
                nameof(SuppliersController.EditSupplierContact),
                ReferencedContactParameters);

            CommonActions.ElementIsDisplayed(SupplierContactObjects.ReferencingSolutionsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierContactObjects.NoReferencingSolutionsInset).Should().BeFalse();
            CommonActions.ElementIsDisplayed(SupplierContactObjects.DeleteLink).Should().BeFalse();

            await RemoveSupplierContactFromSolution();
        }

        [Fact]
        public async Task EditSupplierContact_ReferencedContact_LinkGoesToEditSupplierDetailsForSolution()
        {
            await AddSupplierContactToSolutionIfNotExists();

            NavigateToUrl(
                typeof(SuppliersController),
                nameof(SuppliersController.EditSupplierContact),
                ReferencedContactParameters);

            CommonActions.ClickLinkElement(By.LinkText("Edit"));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditSupplierDetails)).Should().BeTrue();

            await RemoveSupplierContactFromSolution();
        }

        [Fact]
        public void EditSupplierContact_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.ManageSupplierContacts)).Should().BeTrue();
        }

        [Fact]
        public async Task EditSupplierContact_DuplicateDetails_ThrowsError()
        {
            await using var context = GetEndToEndDbContext();

            var supplier = await context.Suppliers.Include(x => x.SupplierContacts)
                .FirstAsync(sc => sc.Id == SupplierId);
            var contact = supplier.SupplierContacts.Last();

            CommonActions.ElementAddValue(SupplierContactObjects.FirstNameInput, contact.FirstName);
            CommonActions.ElementAddValue(SupplierContactObjects.LastNameInput, contact.LastName);
            CommonActions.ElementAddValue(SupplierContactObjects.DepartmentInput, contact.Department);
            CommonActions.ElementAddValue(SupplierContactObjects.PhoneNumberInput, contact.PhoneNumber);
            CommonActions.ElementAddValue(SupplierContactObjects.EmailInput, contact.Email);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.EditSupplierContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                SupplierContactObjects.ContainerError,
                $"Error: {EditContactModelValidator.DuplicateContactErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public async Task EditSupplierContact_ChangeValues_ExpectedResult()
        {
            var firstName = TextGenerators.TextInputAddText(SupplierContactObjects.FirstNameInput, 35);
            var lastName = TextGenerators.TextInputAddText(SupplierContactObjects.LastNameInput, 35);
            var department = TextGenerators.TextInputAddText(SupplierContactObjects.DepartmentInput, 50);
            var phoneNumber = TextGenerators.TextInputAddText(SupplierContactObjects.PhoneNumberInput, 35);
            var email = TextGenerators.EmailInputAddText(SupplierContactObjects.EmailInput, 255);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.ManageSupplierContacts)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var contact = await context.SupplierContacts.FirstAsync(sc => sc.Id == ContactId);

            contact.FirstName.Should().Be(firstName);
            contact.LastName.Should().Be(lastName);
            contact.Department.Should().Be(department);
            contact.PhoneNumber.Should().Be(phoneNumber);
            contact.Email.Should().Be(email);
        }

        private async Task AddSupplierContactToSolutionIfNotExists()
        {
            await using var context = GetEndToEndDbContext();

            var contact = await context.SupplierContacts
                .FirstAsync(sc => sc.Id == ReferencedContactId);

            var solution = await context.CatalogueItems
                .Include(ci => ci.CatalogueItemContacts)
                .FirstAsync(ci => ci.Id == ReferencingSolutionId);

            if (solution.CatalogueItemContacts.Any(x => x.Id == ReferencedContactId)) return;

            solution.CatalogueItemContacts.Add(contact);

            await context.SaveChangesAsync();
        }

        private async Task RemoveSupplierContactFromSolution()
        {
            await using var context = GetEndToEndDbContext();

            var contact = await context.SupplierContacts
                .FirstAsync(sc => sc.Id == ReferencedContactId);

            var solution = await context.CatalogueItems
                .Include(ci => ci.CatalogueItemContacts)
                .FirstAsync(ci => ci.Id == ReferencingSolutionId);

            if (solution.CatalogueItemContacts.All(x => x.Id != ReferencedContactId)) return;

            solution.CatalogueItemContacts.Remove(contact);

            await context.SaveChangesAsync();
        }
    }
}
