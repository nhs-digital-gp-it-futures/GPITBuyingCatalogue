using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    public sealed class ManageSuppliers : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int TargetSupplierId = 99998;

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

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.ActionLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.InactiveSuppliersContainer).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SuppliersTable).Should().BeTrue();
            CommonActions.ElementIsNotDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.InactiveSupplierRow).Should().BeTrue();
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
    }
}
