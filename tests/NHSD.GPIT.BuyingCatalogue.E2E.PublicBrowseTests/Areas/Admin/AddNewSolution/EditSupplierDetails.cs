using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditSupplierDetails : AuthorityTestBase
    {
        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public EditSupplierDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.EditSupplierDetails),
                  Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.Header2)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(EditSupplierDetailsObjects.SupplierContacts)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Submitting_WithNoContactsSelected_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions
                .ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions
                .ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(CommonSelectors.NhsErrorSectionLinkList, "Select a supplier contact")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Submitting_WithMoreThanTwoContactsSelected_ThrowsError()
        {
            CommonActions.ClickAllCheckboxes();

            CommonActions.ClickSave();

            CommonActions
                .ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions
                .ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(CommonSelectors.NhsErrorSectionLinkList, "You can only select up to two supplier contacts")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Submitting_ValidSelection_NavigatesToCorrectPage()
        {
            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();
        }
    }
}
