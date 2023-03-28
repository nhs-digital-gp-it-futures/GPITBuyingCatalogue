using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    [Collection(nameof(AdminCollection))]
    public sealed class ManageSuppliers : AuthorityTestBase
    {
        private const int TargetSupplierId = 99998;
        private const int ActiveSupplierId = 99997;

        public ManageSuppliers(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SuppliersController),
                  nameof(SuppliersController.Index),
                  null)
        {
        }

        [Fact]
        public void ManageSuppliers_AllSectionsDisplayed()
        {
            CommonActions.WaitUntilElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.InactiveSuppliersContainer);

            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.ActionLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchBar).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.InactiveSuppliersContainer).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SuppliersTable).Should().BeTrue();
            CommonActions.ElementIsNotDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.InactiveSupplierRow).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.NoResultsElement).Should().BeFalse();
        }

        [Fact]
        public void ManageSuppliers_ClickHomeBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void ManageSuppliers_ClickAddSupplier_ExpectedResult()
        {
            CommonActions.ClickLinkElement(CommonSelectors.ActionLink);

            CommonActions.PageLoadedCorrectGetIndex(typeof(SuppliersController), nameof(SuppliersController.AddSupplierDetails))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ManageSupplier_ClickEditLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Admin.ManageSuppliers.ManageSuppliers.EditLink, TargetSupplierId.ToString());

            CommonActions.PageLoadedCorrectGetIndex(typeof(SuppliersController), nameof(SuppliersController.EditSupplier))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ManageSupplier_ClickShowInactiveSuppliers_ExpectedResults()
        {
            CommonActions.ElementIsNotDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.InactiveSupplierRow).Should().BeTrue();

            CommonActions.ClickFirstCheckbox();

            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.InactiveSupplierRow).Should().BeTrue();
        }

        [Fact]
        public void ManageSupplier_SearchValid_DisplaysOrders()
        {
            RunTestWithRetry(() =>
            {
                using var context = GetEndToEndDbContext();
                var supplier = context.Suppliers.First();

                CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchBar, supplier.Name);

                CommonActions.WaitUntilElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchListBox);

                CommonActions.ElementExists(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchResult(0))
                    .Should()
                    .BeTrue();

                CommonActions.ElementTextEqualTo(
                    Objects.Admin.ManageSuppliers.ManageSuppliers.SearchResultTitle(0),
                    supplier.Name)
                    .Should()
                    .BeTrue();

                CommonActions.ElementTextEqualTo(
                    Objects.Admin.ManageSuppliers.ManageSuppliers.SearchResultDescription(0),
                    supplier.Id.ToString())
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void ManageSupplier_SearchInvalid_NoResults()
        {
            RunTestWithRetry(() =>
            {
                TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchBar, 5);

                CommonActions.WaitUntilElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchListBox);

                CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.NoResults)
                    .Should()
                    .BeTrue();

                CommonActions.ClickLinkElement(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchButton);

                CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchBar).Should().BeTrue();
                CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.NoResultsElement).Should().BeTrue();
                CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SuppliersTable).Should().BeFalse();
            });
        }

        [Fact]
        public void OrderingDashboard_SearchInvalid_ClickLink_NavigatesCorrectly()
        {
            RunTestWithRetry(() =>
            {
                TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchBar, 5);
                CommonActions.WaitUntilElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchListBox);

                CommonActions.ClickLinkElement(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchButton);
                CommonActions.WaitUntilElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.NoResultsElement);

                CommonActions.ClickLinkElement(ByExtensions.DataTestId("clear-results-link"));

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.Index))
                    .Should().BeTrue();
            });
        }

        [Fact]
        public void ManageSupplier_Search_FiltersTable()
        {
            RunTestWithRetry(() =>
            {
                using var context = GetEndToEndDbContext();
                var supplier = context.Suppliers.First(s => s.Id == ActiveSupplierId && s.IsActive);

                CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchBar, supplier.Name);
                CommonActions.WaitUntilElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchListBox);

                CommonActions.ClickLinkElement(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchButton);

                CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.InactiveSuppliersContainer).Should().BeFalse();
                CommonActions.GetNumberOfTableRowsDisplayed().Should().Be(2); // Count is original item, plus another item that matches the same name
            });
        }

        [Fact]
        public void ManageSupplier_SearchInactiveSupplier_FiltersTable()
        {
            RunTestWithRetry(() =>
            {
                using var context = GetEndToEndDbContext();
                var supplier = context.Suppliers.First(s => !s.IsActive);

                CommonActions.ElementAddValue(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchBar, supplier.Name);
                CommonActions.WaitUntilElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchListBox);

                CommonActions.ClickLinkElement(Objects.Admin.ManageSuppliers.ManageSuppliers.SearchButton);

                CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.InactiveSuppliersContainer).Should().BeFalse();
                CommonActions.GetNumberOfTableRowsDisplayed().Should().Be(1);
            });
        }
    }
}
