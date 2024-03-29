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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ApplicationTypes
{
    [Collection(nameof(AdminCollection))]
    public sealed class DeleteApplicationType : AuthorityTestBase
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { "applicationType", ApplicationType.BrowserBased.ToString() },
        };

        public DeleteApplicationType(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DeleteApplicationTypeController),
                  nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation),
                  Parameters)
        {
        }

        [Theory]
        [InlineData(ApplicationType.BrowserBased)]
        [InlineData(ApplicationType.MobileTablet)]
        [InlineData(ApplicationType.Desktop)]
        public void DeleteApplicationType_CorrectlyDisplayed(ApplicationType applicationType)
        {
            var queryParam = new Dictionary<string, string>
            {
                { "solutionId", SolutionId.ToString() },
                { "applicationType", applicationType.ToString() },
            };

            NavigateToUrl(typeof(DeleteApplicationTypeController), nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation), parameters: queryParam);

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ApplicationTypeObjects.DeleteApplicationTypeCancelLink);
        }

        [Theory]
        [InlineData(ApplicationType.BrowserBased, typeof(BrowserBasedController), nameof(BrowserBasedController.BrowserBased))]
        [InlineData(ApplicationType.MobileTablet, typeof(MobileTabletBasedController), nameof(MobileTabletBasedController.MobileTablet))]
        [InlineData(ApplicationType.Desktop, typeof(DesktopBasedController), nameof(DesktopBasedController.Desktop))]
        public void DeleteApplicationType_ClickGoBackLink_NavigatesToCorrectPage(ApplicationType applicationType, Type controller, string expectedControllerMethod)
        {
            var queryParam = new Dictionary<string, string>
            {
                { "solutionId", SolutionId.ToString() },
                { "applicationType", applicationType.ToString() },
            };

            NavigateToUrl(typeof(DeleteApplicationTypeController), nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation), parameters: queryParam);

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                controller,
                expectedControllerMethod)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(ApplicationType.BrowserBased, typeof(BrowserBasedController), nameof(BrowserBasedController.BrowserBased))]
        [InlineData(ApplicationType.MobileTablet, typeof(MobileTabletBasedController), nameof(MobileTabletBasedController.MobileTablet))]
        [InlineData(ApplicationType.Desktop, typeof(DesktopBasedController), nameof(DesktopBasedController.Desktop))]
        public void DeleteApplicationType_ClickCancelLink_NavigatesToCorrectPage(ApplicationType applicationType, Type controller, string expectedControllerMethod)
        {
            var queryParam = new Dictionary<string, string>
            {
                { "solutionId", SolutionId.ToString() },
                { "applicationType", applicationType.ToString() },
            };

            NavigateToUrl(typeof(DeleteApplicationTypeController), nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation), parameters: queryParam);

            CommonActions.ClickLinkElement(ApplicationTypeObjects.DeleteApplicationTypeCancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                controller,
                expectedControllerMethod)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(ApplicationType.BrowserBased)]
        [InlineData(ApplicationType.MobileTablet)]
        [InlineData(ApplicationType.Desktop)]
        public async Task DeleteApplicationType_DeleteButton_DeletesApplicationType(ApplicationType applicationType)
        {
            await using var context = GetEndToEndDbContext();
            var originalSolution =
                await context.Solutions.AsNoTracking().FirstAsync(x => x.CatalogueItemId == SolutionId);
            var originalApplicationTypeDetail = originalSolution.ApplicationTypeDetail;

            var queryParam = new Dictionary<string, string>
            {
                { "solutionId", SolutionId.ToString() },
                { "applicationType", applicationType.ToString() },
            };

            NavigateToUrl(typeof(DeleteApplicationTypeController), nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation), parameters: queryParam);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ApplicationType))
                .Should().BeTrue();
            var solution = await context.Solutions.AsNoTracking().FirstAsync(s => s.CatalogueItemId == SolutionId);
            var applicationTypeDetail = solution.ApplicationTypeDetail;

            applicationTypeDetail.ApplicationTypes.Should().NotContain(applicationType.AsString(EnumFormat.EnumMemberValue));

            solution.ApplicationTypeDetail = originalApplicationTypeDetail;
            context.Update(solution);
            await context.SaveChangesAsync();
        }
    }
}
