using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    [Collection(nameof(AdminCollection))]
    public sealed class Implementation : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public Implementation(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.Implementation),
                  Parameters)
        {
        }

        [Fact]
        public async Task Implementation_TitleDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.FirstAsync(s => s.Id == SolutionId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Implementation - {solutionName}".FormatForComparison());
        }

        [Fact]
        public async Task Implementation_EnterDescription()
        {
            var implementation = TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);
            solution.ImplementationDetail.Should().Be(implementation);
        }

        [Fact]
        public async Task Implementation_GoBackWithoutSaving()
        {
            TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);

            AdminPages.CommonActions.ClickGoBack();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);
            var implementation = solution.ImplementationDetail;

            implementation.Should().BeNullOrEmpty();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.First(s => s.CatalogueItemId == SolutionId);
            solution.ImplementationDetail = null;
            context.SaveChanges();
        }
    }
}
