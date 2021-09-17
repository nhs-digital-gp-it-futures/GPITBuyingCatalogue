using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    public sealed class DeleteSupplierContact : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int SupplierId = 99998;
        private const int ContactId = 3;
        private const int ContactToDeleteId = 2;

        private static readonly Dictionary<string, string> Parameters =
        new()
        {
            { nameof(SupplierId), SupplierId.ToString() },
            { nameof(ContactId), ContactId.ToString() },
        };

        private static readonly Dictionary<string, string> ContactToDeleteParameters =
        new()
        {
            { nameof(SupplierId), SupplierId.ToString() },
            { nameof(ContactId), ContactToDeleteId.ToString() },
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
        public void DeleteSupplierContact_DeleteContact_ContactDeleted()
        {
            NavigateToUrl(typeof(SuppliersController), nameof(SuppliersController.DeleteSupplierContact), parameters: ContactToDeleteParameters);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(SuppliersController),
            nameof(SuppliersController.ManageSupplierContacts))
            .Should()
            .BeTrue();

            using var context = GetEndToEndDbContext();

            context.SupplierContacts.Count(sc => sc.Id == ContactToDeleteId).Should().Be(0);
        }
    }
}
