using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.DevelopmentPlans
{
    [Collection(nameof(AdminCollection))]
    public sealed class DevelopmentPlans : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public DevelopmentPlans(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DevelopmentPlansController),
                  nameof(DevelopmentPlansController.DevelopmentPlans),
                  Parameters)
        {
        }

        [Fact]
        public async Task DevelopmentPlans_TitleDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.FirstAsync(s => s.Id == SolutionId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Development plans - {solutionName}".FormatForComparison());
        }

        [Fact]
        public void DevelopmentPlans_DisplayedCorrect()
        {
            CommonActions.ElementIsDisplayed(DevelopmentPlanObjects.DevelopmentPlanLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DevelopmentPlanObjects.WorkOffPlansActionLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DevelopmentPlanObjects.WorkOffPlansTable).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task DevelopmentPlans_EnterRoadMapLinkAsync()
        {
            var link = TextGenerators.UrlInputAddText(Objects.Common.CommonSelectors.LinkTextBox, 1000);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);
            solution.RoadMap.Should().Be(link);
        }

        [Fact]
        public async Task DevelopmentPlans_GoBackWithoutSaving()
        {
            TextGenerators.UrlInputAddText(Objects.Common.CommonSelectors.LinkTextBox, 1000);

            AdminPages.CommonActions.ClickGoBack();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);
            var roadmapUrl = solution.RoadMap;

            roadmapUrl.Should().BeNullOrEmpty();
        }

        [Fact]
        public void DevelopmentPlans_ClickAddWorkOffPlan_CorrectRedirect()
        {
            CommonActions.ClickLinkElement(DevelopmentPlanObjects.WorkOffPlansActionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.AddWorkOffPlan))
                .Should()
                .BeTrue();
        }

        public void Dispose()
        {
            ClearRoadMap(SolutionId);
        }
    }
}
