using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes
{
    public sealed class AddClientApplicationType : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddClientApplicationType(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.AddApplicationType),
                  Parameters)
        {
        }

        // Set as Theory to allow for quick extension when other application type views are developed.
        [Theory]
        [InlineData(ClientApplicationType.BrowserBased, nameof(CatalogueSolutionsController.BrowserBased))]
        public void AddClientApplicationType_AddApplicationType(ClientApplicationType clientApplicationType, string expectedControllerMethod)
        {
            CommonActions.ClickRadioButtonWithValue(clientApplicationType.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                expectedControllerMethod)
                .Should().BeTrue();
        }

        [Fact]
        public void AddClientApplicationType_ErrorMessageThrownNoneSelected()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.AddApplicationType))
                .Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(Objects.Common.CommonSelectors.RadioButtonItems, "Select an application type");
        }

        [Fact]
        public void AddClientApplicationType_ClickGoBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ClientApplicationType))
                .Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
