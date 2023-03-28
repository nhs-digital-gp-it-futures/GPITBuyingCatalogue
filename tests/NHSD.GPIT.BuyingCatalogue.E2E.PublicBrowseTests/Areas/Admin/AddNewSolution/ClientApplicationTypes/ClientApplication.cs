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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes
{
    [Collection(nameof(AdminCollection))]
    public sealed class ClientApplication : AuthorityTestBase
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

        public ClientApplication(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.ClientApplicationType),
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
        [InlineData(ClientApplicationType.BrowserBased)]
        [InlineData(ClientApplicationType.Desktop)]
        [InlineData(ClientApplicationType.MobileTablet)]
        public void ExistingClientApplications_Edit_NavigatesCorrectly(ClientApplicationType clientApplicationType)
        {
            NavigateToUrl(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ClientApplicationType),
                ExistingClientApplicationsParameters);

            var formattedApplicationName = FormatApplicationType(clientApplicationType);

            CommonActions.ClickLinkElement(By.XPath($"//tr/td[contains(., '{formattedApplicationName}')]/..//a"));

            (var controllerType, var actionName) = GetExpectedLinkForApplicationType(clientApplicationType);

            CommonActions.PageLoadedCorrectGetIndex(controllerType, actionName)
                .Should()
                .BeTrue();
        }

        private static string FormatApplicationType(ClientApplicationType applicationType) => applicationType switch
        {
            ClientApplicationType.MobileTablet => "Mobile application",
            _ => $"{applicationType.AsString(EnumFormat.DisplayName)} application",
        };

        private static (Type ControllerType, string ActionName) GetExpectedLinkForApplicationType(ClientApplicationType clientApplicationType)
            => clientApplicationType switch
            {
                ClientApplicationType.MobileTablet => (typeof(MobileTabletBasedController), nameof(MobileTabletBasedController.MobileTablet)),
                ClientApplicationType.Desktop => (typeof(DesktopBasedController), nameof(DesktopBasedController.Desktop)),
                ClientApplicationType.BrowserBased => (typeof(BrowserBasedController), nameof(BrowserBasedController.BrowserBased)),
                _ => throw new ArgumentOutOfRangeException(nameof(clientApplicationType)),
            };
    }
}
