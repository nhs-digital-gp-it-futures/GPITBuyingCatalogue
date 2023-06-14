using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ApplicationTypes
{
    [Collection(nameof(AdminCollection))]
    public sealed class AddApplicationType : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddApplicationType(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.AddApplicationType),
                  Parameters)
        {
        }

        [Theory]
        [InlineData(ApplicationType.BrowserBased, typeof(BrowserBasedController), nameof(BrowserBasedController.BrowserBased))]
        [InlineData(ApplicationType.MobileTablet, typeof(MobileTabletBasedController), nameof(MobileTabletBasedController.MobileTablet))]
        [InlineData(ApplicationType.Desktop, typeof(DesktopBasedController), nameof(DesktopBasedController.Desktop))]
        public void AddApplicationType_AddApplicationType(ApplicationType applicationType, Type controller, string expectedControllerMethod)
        {
            CommonActions.ClickRadioButtonWithValue(applicationType.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                controller,
                expectedControllerMethod)
                .Should().BeTrue();
        }

        [Fact]
        public void AddApplicationType_ErrorMessageThrownNoneSelected()
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
        public void AddApplicationType_ClickGoBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ApplicationType))
                .Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
