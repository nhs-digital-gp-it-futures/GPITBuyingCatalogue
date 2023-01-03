using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    public sealed class DeleteSupplierContact : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int SupplierId = 99998;
        private const int ContactId = 3;

        private static readonly Dictionary<string, string> Parameters =
        new()
        {
            { nameof(SupplierId), SupplierId.ToString() },
            { nameof(ContactId), ContactId.ToString() },
        };

        public DeleteSupplierContact(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SuppliersController),
                  nameof(SuppliersController.DeleteSupplierContact),
                  Parameters)
        {
        }

        [Fact]
        public void DeleteSupplierContact_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.DeleteSupplierContactCancelLink).Should().BeTrue();
        }

        [Fact]
        public void DeleteSupplierContact_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplierContact))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void DeleteSupplierContact_ClickCancelLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Admin.ManageSuppliers.ManageSuppliers.DeleteSupplierContactCancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplierContact))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task DeleteSupplierContact_DeleteContact_ContactDeleted()
        {
            var supplierContact = await AddSupplierContact();

            NavigateToUrl(
                typeof(SuppliersController),
                nameof(SuppliersController.DeleteSupplierContact),
                new Dictionary<string, string>
                {
                    { nameof(SupplierId), SupplierId.ToString() },
                    { nameof(ContactId), supplierContact.Id.ToString() },
                });

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.ManageSupplierContacts))
                .Should()
                .BeTrue();

            await using var context = GetEndToEndDbContext();

            context.SupplierContacts.Count(sc => sc.Id == supplierContact.Id).Should().Be(0);
        }

        private async Task<SupplierContact> AddSupplierContact()
        {
            await using var context = GetEndToEndDbContext();

            var supplierContact = new SupplierContact
            {
                SupplierId = 99998,
                FirstName = Strings.RandomString(5),
                LastName = Strings.RandomString(5),
                Email = Strings.RandomEmail(5),
                Department = Strings.RandomString(5),
                LastUpdated = DateTime.UtcNow,
            };

            context.SupplierContacts.Add(supplierContact);

            await context.SaveChangesAsync();

            return supplierContact;
        }
    }
}
