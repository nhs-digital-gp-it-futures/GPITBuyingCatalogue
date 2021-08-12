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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class ImplementationDetails : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ImplementationDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionDetailsController),
                  nameof(SolutionDetailsController.Implementation),
                  Parameters)
        {
        }

        [Fact]
        public async Task ImplementationDetails_ImplementationNameDisplayedAsync()
        {
            await using var context = GetEndToEndDbContext();
            var pageTitle = (await context.CatalogueItems.SingleAsync(s => s.Id == new CatalogueItemId(99999, "001"))).Name;

            CommonActions
            .PageTitle()
            .Should()
            .BeEquivalentTo($"implementation - {pageTitle}".FormatForComparison());
        }

        [Fact]
        public async Task ImplementationDetails_VerifyContent()
        {
            await using var context = GetEndToEndDbContext();
            var info = (await context.Solutions.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "001"))).ImplementationDetail;

            var implementationContent = PublicBrowsePages.SolutionAction.GetSummaryAndDescriptions();

            implementationContent
                .Any(s => s.Contains(info, StringComparison.CurrentCultureIgnoreCase))
                .Should().BeTrue();
        }
    }
}
