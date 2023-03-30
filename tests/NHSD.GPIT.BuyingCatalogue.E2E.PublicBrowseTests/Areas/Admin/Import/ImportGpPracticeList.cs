using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Import;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Import
{
    [Collection(nameof(AdminCollection))]
    public class ImportGpPracticeList : AuthorityTestBase
    {
        public ImportGpPracticeList(LocalWebApplicationFactory factory)
            : base(factory, typeof(ImportController), nameof(ImportController.ImportGpPracticeList), null)
        {
        }

        [Fact]
        public void ImportGpPracticeList_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonObjects.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ImportObjects.CsvUrlInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonObjects.SaveButton).Should().BeTrue();
        }

        [Fact]
        public void ImportGpPracticeList_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonObjects.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void ImportGpPracticeList_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ImportController),
                nameof(ImportController.ImportGpPracticeList)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                ImportObjects.CsvUrlError,
                ImportGpPracticeListModelValidator.CSvUrlErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void ImportGpPracticeList_InvalidUrl_ThrowsError()
        {
            TextGenerators.TextInputAddText(ImportObjects.CsvUrlInput, 20);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ImportController),
                nameof(ImportController.ImportGpPracticeList)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                ImportObjects.CsvUrlError,
                FluentValidationExtensions.InvalidUrlPrefixErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void ImportGpPracticeList_ValidUrl_ExpectedResult()
        {
            CommonActions.ElementAddValue(ImportObjects.CsvUrlInput, "https://www.google.com");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ImportController),
                nameof(ImportController.ImportGpPracticeListConfirmation)).Should().BeTrue();
        }
    }
}
