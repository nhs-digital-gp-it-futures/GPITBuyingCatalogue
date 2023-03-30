using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageSuppliers;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    [Collection(nameof(AdminCollection))]
    public sealed class AddSupplierContact : AuthorityTestBase, IDisposable
    {
        private const int SupplierId = 99996;
        private const int DuplicateSupplierId = 99998;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SupplierId), SupplierId.ToString() },
        };

        private static readonly Dictionary<string, string> DuplicateParameters = new()
        {
            { nameof(SupplierId), DuplicateSupplierId.ToString() },
        };

        public AddSupplierContact(LocalWebApplicationFactory factory)
            : base(factory, typeof(SuppliersController), nameof(SuppliersController.AddSupplierContact), Parameters)
        {
        }

        [Fact]
        public void AddSupplierContact_AllSectionsDisplayed()
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
            CommonActions.ElementIsDisplayed(SupplierContactObjects.NoReferencingSolutionsInset).Should().BeFalse();
            CommonActions.ElementIsDisplayed(SupplierContactObjects.DeleteLink).Should().BeFalse();
        }

        [Fact]
        public void AddSupplierContact_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.ManageSupplierContacts)).Should().BeTrue();
        }

        [Fact]
        public void AddSupplierContact_NoPersonalDetails_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.AddSupplierContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                SupplierContactObjects.FirstNameError,
                ContactModelValidator.PersonalDetailsMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void AddSupplierContact_NoFirstName_ThrowsError()
        {
            TextGenerators.TextInputAddText(SupplierContactObjects.FirstNameInput, 0);
            TextGenerators.TextInputAddText(SupplierContactObjects.LastNameInput, 20);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.AddSupplierContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                SupplierContactObjects.FirstNameError,
                ContactModelValidator.FirstNameMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void AddSupplierContact_NoLastName_ThrowsError()
        {
            TextGenerators.TextInputAddText(SupplierContactObjects.FirstNameInput, 20);
            TextGenerators.TextInputAddText(SupplierContactObjects.LastNameInput, 0);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.AddSupplierContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                SupplierContactObjects.LastNameError,
                ContactModelValidator.LastNameMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void AddSupplierContact_NoContactDetails_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.AddSupplierContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                SupplierContactObjects.PhoneNumberError,
                ContactModelValidator.ContactDetailsMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void AddSupplierContact_InputIncorrectEmailValue_ThrowsError()
        {
            TextGenerators.TextInputAddText(SupplierContactObjects.EmailInput, 255);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.AddSupplierContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                SupplierContactObjects.EmailError,
                "Enter an email address in the correct format, like name@example.com").Should().BeTrue();
        }

        [Fact]
        public async Task AddSupplierContact_DuplicateDetails_ThrowsError()
        {
            NavigateToUrl(
                typeof(SuppliersController),
                nameof(SuppliersController.AddSupplierContact),
                DuplicateParameters);

            await using var context = GetEndToEndDbContext();

            var contact = await context.SupplierContacts.FirstAsync(sc => sc.SupplierId == DuplicateSupplierId);

            CommonActions.ElementAddValue(SupplierContactObjects.FirstNameInput, contact.FirstName);
            CommonActions.ElementAddValue(SupplierContactObjects.LastNameInput, contact.LastName);
            CommonActions.ElementAddValue(SupplierContactObjects.DepartmentInput, contact.Department);
            CommonActions.ElementAddValue(SupplierContactObjects.PhoneNumberInput, contact.PhoneNumber);
            CommonActions.ElementAddValue(SupplierContactObjects.EmailInput, contact.Email);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.AddSupplierContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                SupplierContactObjects.ContainerError,
                "Error: A contact with these contact details already exists for this supplier").Should().BeTrue();
        }

        [Fact]
        public void AddSupplierContact_CorrectInput_ExpectedResult()
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

            Driver.FindElements(CommonSelectors.TableRow).Count.Should().Be(1);

            GetEndToEndDbContext()
                .SupplierContacts.AsNoTracking()
                .Any(
                    x => x.FirstName == firstName && x.LastName == lastName && x.Department == department
                        && x.PhoneNumber == phoneNumber && x.Email == email)
                .Should()
                .BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.EditSupplier)).Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                Framework.Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactStatus,
                "Completed").Should().BeTrue();
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
