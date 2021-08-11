using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class Description : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public Description(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.Description),
                  Parameters)
        {
        }

        [Fact]
        public async Task Description_TitleDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.SingleAsync(s => s.Id == SolutionId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Description - {solutionName}".FormatForComparison());
        }

        [Fact]
        public async Task Description_EnterAllFieldsAsync()
        {
            var summary = TextGenerators.TextInputAddText(CommonSelectors.Summary, 350);
            var fullDescription = TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);
            var link = TextGenerators.UrlInputAddText(Objects.Common.CommonSelectors.LinkTextBox, 1000);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.CatalogueItemId == SolutionId);

            solution.Summary.Should().Be(summary);
            solution.FullDescription.Should().Be(fullDescription);
            solution.AboutUrl.Should().Be(link);
        }

        [Fact]
        public async Task Description_GoBackWithoutSaving()
        {
            TextGenerators.TextInputAddText(CommonSelectors.Description, 1000);
            TextGenerators.UrlInputAddText(Objects.Common.CommonSelectors.LinkTextBox, 1000);

            AdminPages.CommonActions.ClickGoBack();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.CatalogueItemId == SolutionId);

            solution.FullDescription.Should().BeNullOrEmpty();
            solution.AboutUrl.Should().BeNullOrEmpty();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.CatalogueItemId == SolutionId);
            solution.FullDescription = string.Empty;
            solution.AboutUrl = string.Empty;
            context.SaveChanges();
        }
    }
}
