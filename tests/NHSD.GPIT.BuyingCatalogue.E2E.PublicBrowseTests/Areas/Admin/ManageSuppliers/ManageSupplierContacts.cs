using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    [Collection(nameof(AdminCollection))]
    public sealed class ManageSupplierContacts : AuthorityTestBase
    {
        private const int SupplierId = 99998;

        private static readonly Dictionary<string, string> Parameters = new() { { nameof(SupplierId), SupplierId.ToString() } };

        public ManageSupplierContacts(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SuppliersController),
                  nameof(SuppliersController.ManageSupplierContacts),
                  Parameters)
        {
        }

        [Fact]
        public void ManageSupplierContacts_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.ActionLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.ManageSupplierContactsTable)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.ManageSupplierContinueButton)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ManageSupplierContacts_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(typeof(SuppliersController), nameof(SuppliersController.EditSupplier))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ManageSuppliersContacts_ClickAddContact_ExpectedResult()
        {
            CommonActions.ClickLinkElement(CommonSelectors.ActionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.AddSupplierContact))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task ManageSupplierContacts_ClickEditContact_ExpectedResult()
        {
            await using var context = GetEndToEndDbContext();

            var contact = await context.SupplierContacts.FirstAsync(sc => sc.SupplierId == SupplierId);

            var @params = new Dictionary<string, string>
            {
                { nameof(SupplierId), SupplierId.ToString() },
                { "contactid", contact.Id.ToString() },
            };

            CommonActions.ClickLinkElement(
                Objects.Admin.ManageSuppliers.ManageSuppliers.ManageSupplierContactsEditLink,
                UrlGenerator.GenerateUrlFromMethod(typeof(SuppliersController), nameof(SuppliersController.EditSupplierContact), @params));

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplierContact))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ManageSupplierContacts_ClickContinueButton_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Admin.ManageSuppliers.ManageSuppliers.ManageSupplierContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplier))
                .Should()
                .BeTrue();
        }
    }
}
