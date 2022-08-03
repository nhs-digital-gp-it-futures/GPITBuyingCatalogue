using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Suppliers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    public sealed class EditSupplier : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int SupplierId = 99998;
        private const int InactiveSupplierId = 99996;

        private static readonly Dictionary<string, string> Parameters = new() { { nameof(SupplierId), SupplierId.ToString() } };

        private static readonly Dictionary<string, string> InactiveSupplierParameters = new() { { nameof(SupplierId), InactiveSupplierId.ToString() } };

        public EditSupplier(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SuppliersController),
                  nameof(SuppliersController.EditSupplier),
                  Parameters)
        {
        }

        [Fact]
        public void EditSupplier_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierDetailsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactsLink).Should().BeTrue();
            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(2);
        }

        [Fact]
        public void EditSupplier_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(typeof(SuppliersController), nameof(SuppliersController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditSupplier_ClickEditDetailsLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierDetailsLink);

            CommonActions.PageLoadedCorrectGetIndex(typeof(SuppliersController), nameof(SuppliersController.EditSupplierDetails))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditSupplier_ClickEditAddressLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressLink);

            CommonActions.PageLoadedCorrectGetIndex(typeof(SuppliersController), nameof(SuppliersController.EditSupplierAddress))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditSupplier_ClickEditContactsLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierContactsLink);

            CommonActions.PageLoadedCorrectGetIndex(typeof(SuppliersController), nameof(SuppliersController.ManageSupplierContacts))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditSupplier_InactiveSupplier_TryToSetActive_NotAllMandatoryFieldsCompleted_ThrowsError()
        {
            NavigateToUrl(typeof(SuppliersController), nameof(SuppliersController.EditSupplier), parameters: InactiveSupplierParameters);

            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplier))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                Objects.Admin.EditSupplierObjects.MandatorySectionsMissingError)
                .Should()
                .BeTrue();
        }
    }
}
