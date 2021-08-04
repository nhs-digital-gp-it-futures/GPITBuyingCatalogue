using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class Interoperability : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            {
                nameof(SolutionId), SolutionId.ToString()
            },
        };

        public Interoperability(LocalWebApplicationFactory factory)
            : base(factory, typeof(CatalogueSolutionsController), nameof(CatalogueSolutionsController.Interoperability), Parameters)
        {
        }

        [Fact]
        public async Task Interoperability_SaveLink()
        {
            var url = TextGenerators.UrlInputAddText(Objects.Common.CommonSelectors.LinkTextBox, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == SolutionId);

            solution.IntegrationsUrl.Should().Be(url);
        }

        [Fact]
        public async Task Interoperability_GoBackDoesNotSave()
        {
            TextGenerators.UrlInputAddText(Objects.Common.CommonSelectors.LinkTextBox, 1000);

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == SolutionId);

            solution.IntegrationsUrl.Should().BeNullOrWhiteSpace();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == SolutionId);
            solution.Integrations = null;
            solution.IntegrationsUrl = null;
            context.SaveChanges();
        }
    }
}
