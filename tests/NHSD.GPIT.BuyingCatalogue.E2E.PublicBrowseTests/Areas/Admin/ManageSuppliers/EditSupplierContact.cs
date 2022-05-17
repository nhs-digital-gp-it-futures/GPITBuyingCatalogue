using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    public sealed class EditSupplierContact : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int SupplierId = 99998;
        private const int ContactId = 3;
        private const int ReferencedContactId = 2;
        private const int OtherContactId = 2;
        private static readonly CatalogueItemId ReferencingSolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(SupplierId), SupplierId.ToString() },
                { nameof(ContactId), ContactId.ToString() },
            };

        private static readonly Dictionary<string, string> ReferencedContactParameters =
            new()
            {
                { nameof(SupplierId), SupplierId.ToString() },
                { nameof(ContactId), ReferencedContactId.ToString() },
            };

        public EditSupplierContact(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SuppliersController),
                  nameof(SuppliersController.EditSupplierContact),
                  Parameters)
        {
        }

        [Fact]
        public void EditSupplierContact_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactFirstName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactLastName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactDepartment).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactPhoneNumber).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactEmail).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactReferencingSolutionsTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactNoReferencingSolutionsInset).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactDeleteLink).Should().BeTrue();
        }

        [Fact]
        public void EditSupplierContact_ReferencedContact_SectionsDisplayed()
        {
            AddSupplierContactToSolutionIfNotExists();

            NavigateToUrl(typeof(SuppliersController), nameof(SuppliersController.EditSupplierContact), ReferencedContactParameters);

            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactReferencingSolutionsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactNoReferencingSolutionsInset).Should().BeFalse();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactDeleteLink).Should().BeFalse();
        }

        [Fact]
        public void EditSupplierContact_ReferencedConstact_LinkGoesToEditSupplierDetailsForSolution()
        {
            AddSupplierContactToSolutionIfNotExists();

            NavigateToUrl(typeof(SuppliersController), nameof(SuppliersController.EditSupplierContact), ReferencedContactParameters);

            CommonActions.ClickLinkElement(By.LinkText("Edit"));

            CommonActions.PageLoadedCorrectGetIndex(typeof(CatalogueSolutionsController), nameof(CatalogueSolutionsController.EditSupplierDetails))
                .Should().BeTrue();
        }

        [Fact]
        public void EditSupplierContact_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.ManageSupplierContacts))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task EditSupplierContact_DuplicateDetails_ThrowsError()
        {
            await using var context = GetEndToEndDbContext();

            var contact = await context.SupplierContacts.FirstAsync(sc => sc.Id == OtherContactId);

            CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactFirstName, contact.FirstName);
            CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactLastName, contact.LastName);
            CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactDepartment, contact.Department);
            CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactPhoneNumber, contact.PhoneNumber);
            CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactEmail, contact.Email);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplierContact))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                    Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactContainerError,
                    "Error: A contact with these contact details already exists for this supplier")
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task EditSupplierContact_ChangeValues_ExpectedResult()
        {
            var firstName = TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactFirstName, 35);
            var lastName = TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactLastName, 35);
            var department = TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactDepartment, 50);
            var phoneNumber = TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactPhoneNumber, 35);
            var email = TextGenerators.EmailInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactEmail, 255);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.ManageSupplierContacts))
                .Should()
                .BeTrue();

            await using var context = GetEndToEndDbContext();

            var contact = await context.SupplierContacts.SingleAsync(sc => sc.Id == ContactId);

            contact.FirstName.Should().Be(firstName);
            contact.LastName.Should().Be(lastName);
            contact.Department.Should().Be(department);
            contact.PhoneNumber.Should().Be(phoneNumber);
            contact.Email.Should().Be(email);
        }

        private async void AddSupplierContactToSolutionIfNotExists()
        {
            using var context = GetEndToEndDbContext();

            var contact = await context.SupplierContacts.SingleAsync(sc => sc.Id == ReferencedContactId);

            var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == ReferencingSolutionId);

            if (!solution.CatalogueItemContacts.Contains(contact))
            {
                solution.CatalogueItemContacts.Add(contact);

                await context.SaveChangesAsync();
            }
        }
    }
}
