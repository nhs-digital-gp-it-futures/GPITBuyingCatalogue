using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    public sealed class AddSupplierContact : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int SupplierId = 99996;

        private const int DuplicateSupplierId = 99998;

        private static readonly Dictionary<string, string> Parameters = new() { { nameof(SupplierId), SupplierId.ToString() } };

        private static readonly Dictionary<string, string> DuplicateParameters = new() { { nameof(SupplierId), DuplicateSupplierId.ToString() } };

        public AddSupplierContact(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SuppliersController),
                  nameof(SuppliersController.AddSupplierContact),
                  Parameters)
        {
        }

        [Fact]
        public void AddSupplierContact_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactFirstName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactLastName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactDepartment).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactPhoneNumber).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactEmail).Should().BeTrue();
        }

        [Fact]
        public void AddSupplierContact_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.ManageSupplierContacts))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddSupplierContact_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.AddSupplierContact))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                    Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactFirstNameError,
                    "Enter a first name")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                    Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactLastNameError,
                    "Enter a last name")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                    Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactDepartmentError,
                    "Enter a department name")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                    Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactPhoneNumberError,
                    "Enter a phone number")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                    Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactEmailError,
                    "Enter an email address")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddSupplierContact_InputIncorrectEmailValue_ThrowsError()
        {
            TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactEmail, 255);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.AddSupplierContact))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                    Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactEmailError,
                    "Enter an email address in the correct format, like name@example.com")
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddSupplierContact_DuplicateDetails_ThrowsError()
        {
            NavigateToUrl(typeof(SuppliersController), nameof(SuppliersController.AddSupplierContact), DuplicateParameters);

            await using var context = GetEndToEndDbContext();

            var contact = await context.SupplierContacts.FirstAsync(sc => sc.SupplierId == DuplicateSupplierId);

            CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactFirstName, contact.FirstName);
            CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactLastName, contact.LastName);
            CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactDepartment, contact.Department);
            CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactPhoneNumber, contact.PhoneNumber);
            CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactEmail, contact.Email);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.AddSupplierContact))
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
        public async Task AddSupplierContact_CorrectInput_ExpectedResult()
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

            Driver.FindElements(CommonSelectors.TableRow).Count.Should().Be(1);

            await using var context = GetEndToEndDbContext();

            var contact = await context.SupplierContacts.OrderByDescending(sc => sc.Id).FirstAsync();

            contact.FirstName.Should().Be(firstName);
            contact.LastName.Should().Be(lastName);
            contact.Department.Should().Be(department);
            contact.PhoneNumber.Should().Be(phoneNumber);
            contact.Email.Should().Be(email);

            CommonActions.ClickLinkElement(Objects.Admin.ManageSuppliers.ManageSuppliers.ManageSupplierContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplier))
                .Should()
                .BeTrue();

            CommonActions.ElementTextEqualTo(
                    Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactStatus,
                    "Completed")
                .Should()
                .BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var supplier = context.Suppliers.First(sc => sc.Id == SupplierId);

            supplier.SupplierContacts.Clear();

            context.SaveChanges();
        }
    }
}
