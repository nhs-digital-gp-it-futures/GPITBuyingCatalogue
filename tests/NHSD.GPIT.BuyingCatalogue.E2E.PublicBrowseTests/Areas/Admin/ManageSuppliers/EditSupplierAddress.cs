using System.Collections.Generic;
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
    public sealed class EditSupplierAddress : AuthorityTestBase
    {
        private const int SupplierId = 99996;

        private static readonly Dictionary<string, string> Parameters = new() { { nameof(SupplierId), SupplierId.ToString() } };

        public EditSupplierAddress(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SuppliersController),
                  nameof(SuppliersController.EditSupplierAddress),
                  Parameters)
        {
        }

        [Fact]
        public void EditSupplierAddress_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressAddressLine1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressAddressLine2).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressAddressLine3).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressAddressLine4).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressAddressLine5).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressTown).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressCounty).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressPostcode).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressCountry).Should().BeTrue();
        }

        [Fact]
        public void EditSupplierAddress_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplier))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditSupplierAddress_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplierAddress))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressAddressLine1Error,
                "Enter a building or street").Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressTownError,
                "Enter a town or city").Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressPostcodeError,
                "Enter a postcode").Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressCountryError,
                "Enter a country").Should().BeTrue();
        }

        [Fact]
        public void EditSupplierAddress_CorrectInput_ExpectedResult()
        {
            TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressAddressLine1, 50);
            TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressTown, 60);
            TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressPostcode, 10);
            TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressCountry, 60);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(SuppliersController), nameof(SuppliersController.EditSupplier))
                .Should()
                .BeTrue();

            CommonActions.ElementTextEqualTo(
                    Objects.Admin.ManageSuppliers.ManageSuppliers.EditSupplierAddressStatus,
                    "Completed")
                .Should()
                .BeTrue();
        }
    }
}
