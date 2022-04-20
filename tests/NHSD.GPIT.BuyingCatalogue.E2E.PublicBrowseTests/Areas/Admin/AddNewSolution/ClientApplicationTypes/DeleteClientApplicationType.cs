﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnumsNET;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.EditSolution;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes
{
    public sealed class DeleteClientApplicationType : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { "applicationType", ClientApplicationType.BrowserBased.ToString() },
        };

        public DeleteClientApplicationType(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DeleteApplicationTypeController),
                  nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation),
                  Parameters)
        {
        }

        [Theory]
        [InlineData(ClientApplicationType.BrowserBased)]
        [InlineData(ClientApplicationType.MobileTablet)]
        [InlineData(ClientApplicationType.Desktop)]
        public void DeleteClientApplicationType_CorrectlyDisplayed(ClientApplicationType clientApplicationType)
        {
            var queryParam = new Dictionary<string, string>
            {
                { "solutionId", SolutionId.ToString() },
                { "applicationType", clientApplicationType.ToString() },
            };

            NavigateToUrl(typeof(DeleteApplicationTypeController), nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation), parameters: queryParam);

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ClientApplicationObjects.DeleteClientApplicationCancelLink);
        }

        [Theory]
        [InlineData(ClientApplicationType.BrowserBased, typeof(BrowserBasedController), nameof(BrowserBasedController.BrowserBased))]
        [InlineData(ClientApplicationType.MobileTablet, typeof(MobileTabletBasedController), nameof(MobileTabletBasedController.MobileTablet))]
        [InlineData(ClientApplicationType.Desktop, typeof(DesktopBasedController), nameof(DesktopBasedController.Desktop))]
        public void DeleteClientApplicationType_ClickGoBackLink_NavigatesToCorrectPage(ClientApplicationType clientApplicationType, Type controller, string expectedControllerMethod)
        {
            var queryParam = new Dictionary<string, string>
            {
                { "solutionId", SolutionId.ToString() },
                { "applicationType", clientApplicationType.ToString() },
            };

            NavigateToUrl(typeof(DeleteApplicationTypeController), nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation), parameters: queryParam);

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                controller,
                expectedControllerMethod)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(ClientApplicationType.BrowserBased, typeof(BrowserBasedController), nameof(BrowserBasedController.BrowserBased))]
        [InlineData(ClientApplicationType.MobileTablet, typeof(MobileTabletBasedController), nameof(MobileTabletBasedController.MobileTablet))]
        [InlineData(ClientApplicationType.Desktop, typeof(DesktopBasedController), nameof(DesktopBasedController.Desktop))]
        public void DeleteClientApplicationType_ClickCancelLink_NavigatesToCorrectPage(ClientApplicationType clientApplicationType, Type controller, string expectedControllerMethod)
        {
            var queryParam = new Dictionary<string, string>
            {
                { "solutionId", SolutionId.ToString() },
                { "applicationType", clientApplicationType.ToString() },
            };

            NavigateToUrl(typeof(DeleteApplicationTypeController), nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation), parameters: queryParam);

            CommonActions.ClickLinkElement(ClientApplicationObjects.DeleteClientApplicationCancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                controller,
                expectedControllerMethod)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(ClientApplicationType.BrowserBased)]
        [InlineData(ClientApplicationType.MobileTablet)]
        [InlineData(ClientApplicationType.Desktop)]
        public async Task DeleteClientApplicationType_DeleteButton_DeletesClientApplicationType(ClientApplicationType clientApplicationType)
        {
            var queryParam = new Dictionary<string, string>
            {
                { "solutionId", SolutionId.ToString() },
                { "applicationType", clientApplicationType.ToString() },
            };

            NavigateToUrl(typeof(DeleteApplicationTypeController), nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation), parameters: queryParam);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ClientApplicationType))
                .Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.CatalogueItemId == SolutionId);
            var clientApplication = solution.GetClientApplication();

            clientApplication.ClientApplicationTypes.Should().NotContain(clientApplicationType.AsString(EnumFormat.EnumMemberValue));
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
