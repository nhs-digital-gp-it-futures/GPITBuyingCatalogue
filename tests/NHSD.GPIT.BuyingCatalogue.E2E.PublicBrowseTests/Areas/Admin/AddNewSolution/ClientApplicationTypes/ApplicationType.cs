using System;
using System.Collections.Generic;
using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ApplicationTypes
{
    [Collection(nameof(AdminCollection))]
    public sealed class ApplicationType : AuthorityTestBase
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");
        private static readonly CatalogueItemId SolutionWithApplicationTypes = new CatalogueItemId(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        private static readonly Dictionary<string, string> ExistingApplicationTypesParameters = new()
        {
            { nameof(SolutionId), SolutionWithApplicationTypes.ToString() },
        };

        public ApplicationType(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.ApplicationType),
                  Parameters)
        {
        }

        [Fact]
        public void ApplicationType_ClickAddLink()
        {
            CommonActions.ClickLinkElement(Objects.Admin.CommonObjects.ActionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.AddApplicationType))
                .Should().BeTrue();
        }

        [Fact]
        public void ApplicationType_ClickBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(ServiceContracts.Solutions.ApplicationType.BrowserBased)]
        [InlineData(ServiceContracts.Solutions.ApplicationType.Desktop)]
        [InlineData(ServiceContracts.Solutions.ApplicationType.MobileTablet)]
        public void ExistingApplicationTypes_Edit_NavigatesCorrectly(ServiceContracts.Solutions.ApplicationType applicationType)
        {
            NavigateToUrl(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ApplicationType),
                ExistingApplicationTypesParameters);

            var formattedApplicationName = FormatApplicationType(applicationType);

            CommonActions.ClickLinkElement(By.XPath($"//tr/td[contains(., '{formattedApplicationName}')]/..//a"));

            (var controllerType, var actionName) = GetExpectedLinkForApplicationType(applicationType);

            CommonActions.PageLoadedCorrectGetIndex(controllerType, actionName)
                .Should()
                .BeTrue();
        }

        private static string FormatApplicationType(ServiceContracts.Solutions.ApplicationType applicationType) => applicationType switch
        {
            ServiceContracts.Solutions.ApplicationType.MobileTablet => "Mobile application",
            _ => $"{applicationType.AsString(EnumFormat.DisplayName)} application",
        };

        private static (Type ControllerType, string ActionName) GetExpectedLinkForApplicationType(ServiceContracts.Solutions.ApplicationType applicationType)
            => applicationType switch
            {
                ServiceContracts.Solutions.ApplicationType.MobileTablet => (typeof(MobileTabletBasedController), nameof(MobileTabletBasedController.MobileTablet)),
                ServiceContracts.Solutions.ApplicationType.Desktop => (typeof(DesktopBasedController), nameof(DesktopBasedController.Desktop)),
                ServiceContracts.Solutions.ApplicationType.BrowserBased => (typeof(BrowserBasedController), nameof(BrowserBasedController.BrowserBased)),
                _ => throw new ArgumentOutOfRangeException(nameof(applicationType)),
            };
    }
}
