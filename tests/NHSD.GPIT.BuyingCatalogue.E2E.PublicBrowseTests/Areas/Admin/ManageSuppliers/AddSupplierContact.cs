﻿using System;
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
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    public sealed class AddSupplierContact : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
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
        public void AddSupplierContact_NoInputs_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SuppliersController),
                nameof(SuppliersController.AddSupplierContact)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                SupplierContactObjects.FirstNameError).Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                SupplierContactObjects.PhoneNumberError).Should().BeTrue();
        }

        [Fact]
        public async Task AddSupplierContact_CorrectInput_ExpectedResult()
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

            await using var context = GetEndToEndDbContext();

            var contact = await context.SupplierContacts.OrderByDescending(sc => sc.Id).FirstAsync();

            contact.FirstName.Should().Be(firstName);
            contact.LastName.Should().Be(lastName);
            contact.Department.Should().Be(department);
            contact.PhoneNumber.Should().Be(phoneNumber);
            contact.Email.Should().Be(email);

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
