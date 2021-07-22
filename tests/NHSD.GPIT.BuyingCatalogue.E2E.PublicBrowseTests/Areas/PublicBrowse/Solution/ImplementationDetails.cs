using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class ImplementationDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ImplementationDetails(LocalWebApplicationFactory factory)
            : base(factory, "solutions/futures/99999-001/implementation")
        {
        }

        [Fact]
        public async Task ImplementationDetails_ImplementationNameDisplayedAsync()
        {
            await using var context = GetEndToEndDbContext();
            var pageTitle = (await context.CatalogueItems.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "001"))).Name;

            CommonActions
            .PageTitle()
            .Should()
            .BeEquivalentTo($"implementation - {pageTitle}".FormatForComparison());
        }

        [Fact]
        public async Task ImplementationDetails_VerifyContent()
        {
            await using var context = GetEndToEndDbContext();
            var info = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "001"))).ImplementationDetail;

            var implementationContent = PublicBrowsePages.SolutionAction.GetSummaryAndDescriptions();

            implementationContent
                .Any(s => s.Contains(info, StringComparison.CurrentCultureIgnoreCase))
                .Should().BeTrue();
        }
    }
}
