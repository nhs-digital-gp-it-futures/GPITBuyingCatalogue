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
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class Details : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
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
        public async Task Details_TitleDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.SingleAsync(s => s.Id == SolutionId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Details - {solutionName}".FormatForComparison());
        }

        [Fact]
        public async Task Details_IsFoundationUpdatedCorrectly()
        {
            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickCheckboxByLabel("Foundation Solution");
            CommonActions.ClickCheckboxByLabel("DFOCVC Framework");
            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems
                .Include(s => s.Solution)
                .Include(s => s.Solution.FrameworkSolutions)
                .SingleAsync(s => s.Id == SolutionId);

            var solutions = solution.Solution.FrameworkSolutions.ToList();
            solutions.Count.Should().Be(1);
            solutions[0].FrameworkId.Should().Be("NHSDGP001");
            solutions[0].IsFoundation.Should().Be(true);
        }

        [Fact]
        public async Task Details_FrameworkDFOCVCSelectionUpdatedCorrectly()
        {
            ResetSelectedFramework("NHSDGP001");

            Driver.Navigate().Refresh();

            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickCheckboxByLabel("DFOCVC Framework");
            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems
                .Include(s => s.Solution)
                .Include(s => s.Solution.FrameworkSolutions)
                .SingleAsync(s => s.Id == SolutionId);

            var solutions = solution.Solution.FrameworkSolutions.ToList();
            solutions.Count.Should().Be(1);
            solutions[0].FrameworkId.Should().Be("DFOCVC001");
            solutions[0].IsFoundation.Should().Be(false);
        }

        [Fact]
        public async Task Details_NotFoundationUpdatedCorrectly()
        {
            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickCheckboxByLabel("DFOCVC Framework");
            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems
                .Include(s => s.Solution)
                .Include(s => s.Solution.FrameworkSolutions)
                .SingleAsync(s => s.Id == SolutionId);

            var solutions = solution.Solution.FrameworkSolutions.ToList();
            solutions.Count.Should().Be(1);
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
                .Single(s => s.Id == SolutionId);

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
