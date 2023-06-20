using System;
using System.Collections.Generic;
using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes
{
    [Collection(nameof(AdminCollection))]
    public sealed class ApplicationTypes : AuthorityTestBase
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");
        private static readonly CatalogueItemId SolutionWithClientApplications = new CatalogueItemId(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        private static readonly Dictionary<string, string> ExistingClientApplicationsParameters = new()
        {
            { nameof(SolutionId), SolutionWithClientApplications.ToString() },
        };

        public ApplicationTypes(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.ApplicationType),
                  Parameters)
        {
        }

        [Fact]
        public void ClientApplication_ClickAddLink()
        {
            CommonActions.ClickLinkElement(Objects.Admin.CommonObjects.ActionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.AddApplicationType))
                .Should().BeTrue();
        }

        [Fact]
        public void ClientApplication_ClickBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(ApplicationType.BrowserBased)]
        [InlineData(ApplicationType.Desktop)]
        [InlineData(ApplicationType.MobileTablet)]
        public void ExistingClientApplications_Edit_NavigatesCorrectly(ApplicationType clientApplicationType)
        {
            NavigateToUrl(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ApplicationType),
                ExistingClientApplicationsParameters);

            var formattedApplicationName = FormatApplicationType(clientApplicationType);

            CommonActions.ClickLinkElement(By.XPath($"//tr/td[contains(., '{formattedApplicationName}')]/..//a"));

            (var controllerType, var actionName) = GetExpectedLinkForApplicationType(clientApplicationType);

            CommonActions.PageLoadedCorrectGetIndex(controllerType, actionName)
                .Should()
                .BeTrue();
        }

        private static string FormatApplicationType(ApplicationType applicationType) => applicationType switch
        {
            ApplicationType.MobileTablet => "Mobile application",
            _ => $"{applicationType.AsString(EnumFormat.DisplayName)} application",
        };

        private static (Type ControllerType, string ActionName) GetExpectedLinkForApplicationType(ApplicationType clientApplicationType)
            => clientApplicationType switch
            {
                ApplicationType.MobileTablet => (typeof(MobileTabletBasedController), nameof(MobileTabletBasedController.MobileTablet)),
                ApplicationType.Desktop => (typeof(DesktopBasedController), nameof(DesktopBasedController.Desktop)),
                ApplicationType.BrowserBased => (typeof(BrowserBasedController), nameof(BrowserBasedController.BrowserBased)),
                _ => throw new ArgumentOutOfRangeException(nameof(clientApplicationType)),
            };
    }
}
