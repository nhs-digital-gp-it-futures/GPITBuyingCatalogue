using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    [Collection(nameof(AdminCollection))]
    public sealed class Details : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public Details(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.Details),
                  Parameters)
        {
        }

        [Fact]
        public async Task Details_SectionsDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.FirstAsync(s => s.Id == SolutionId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Details - {solutionName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(By.Id("is-pilot-solution")).Should().BeTrue();
        }

        [Fact]
        public async Task IsPilotSolution_UpdatesSolutionCorrectly()
        {
            CommonActions.ClickCheckboxByLabel("Yes, this solution is available for pilot");

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions
                .FirstAsync(s => s.CatalogueItemId == SolutionId);

            solution.IsPilotSolution.Should().BeTrue();
        }

        [Fact]
        public async Task Details_IsFoundationUpdatedCorrectly()
        {
            CommonActions.ClickCheckboxByLabel("GP IT Futures Framework");
            CommonActions.ClickCheckboxByLabel("Foundation Solution");
            CommonActions.ClickCheckboxByLabel("DFOCVC Framework");
            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems
                .Include(s => s.Solution)
                .Include(s => s.Solution.FrameworkSolutions)
                .FirstAsync(s => s.Id == SolutionId);

            var solutions = solution.Solution.FrameworkSolutions.ToList();
            solutions[0].FrameworkId.Should().Be("NHSDGP001");
            solutions[0].IsFoundation.Should().Be(true);
        }

        [Fact]
        public async Task Details_FrameworkDFOCVCSelectionUpdatedCorrectly()
        {
            ResetSelectedFramework("NHSDGP001");

            Driver.Navigate().Refresh();

            CommonActions.ClickCheckboxByLabel("GP IT Futures Framework");
            CommonActions.ClickCheckboxByLabel("DFOCVC Framework");
            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems
                .AsNoTracking()
                .Include(s => s.Solution)
                .Include(s => s.Solution.FrameworkSolutions)
                .FirstAsync(s => s.Id == SolutionId);

            var solutions = solution.Solution.FrameworkSolutions.ToList();
            solutions[0].FrameworkId.Should().Be("DFOCVC001");
            solutions[0].IsFoundation.Should().Be(false);
        }

        [Fact]
        public async Task Details_NotFoundationUpdatedCorrectly()
        {
            CommonActions.ClickCheckboxByLabel("GP IT Futures Framework");
            CommonActions.ClickCheckboxByLabel("DFOCVC Framework");
            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems
                .AsNoTracking()
                .Include(s => s.Solution)
                .Include(s => s.Solution.FrameworkSolutions)
                .FirstAsync(s => s.Id == SolutionId);

            var solutions = solution.Solution.FrameworkSolutions.ToList();
            solutions[0].FrameworkId.Should().Be("NHSDGP001");
            solutions[0].IsFoundation.Should().Be(false);
        }

        public void Dispose()
        {
            ResetSelectedFramework("DFOCVC001");
        }

        private void ResetSelectedFramework(string framework)
        {
            using var context = GetEndToEndDbContext();
            var catalogueItem = context.CatalogueItems
                .Include(s => s.Solution)
                .Include(s => s.Solution.FrameworkSolutions)
                .First(s => s.Id == SolutionId);

            catalogueItem.Solution.FrameworkSolutions.Clear();
            catalogueItem.Solution.FrameworkSolutions.Add(new EntityFramework.Catalogue.Models.FrameworkSolution
            {
                FrameworkId = framework,
                IsFoundation = false,
                LastUpdated = DateTime.Now,
                SolutionId = catalogueItem.Id,
            });

            context.SaveChanges();
        }
    }
}
