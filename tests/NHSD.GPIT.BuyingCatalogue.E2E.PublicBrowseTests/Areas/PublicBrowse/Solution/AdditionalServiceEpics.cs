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
    public sealed class AdditionalServiceEpics : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly Guid CapabilityId = Guid.Parse("4F09E77B-E3A3-4A25-8EC1-815921F83628");

        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly CatalogueItemId AdditionalServiceId = new(99999, "001A999");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
            { nameof(CapabilityId), CapabilityId.ToString() },
        };

        public AdditionalServiceEpics(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionDetailsController),
                  nameof(SolutionDetailsController.CheckEpicsAdditionalServices),
                  Parameters)
        {
        }

        [Fact]
        public async Task AdditionalServiceEpics_CapabilityNameDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var capabilityName = (await context.Capabilities.SingleAsync(c => c.Id == CapabilityId)).Name;
            var solutionName = (await context.CatalogueItems.SingleAsync(c => c.CatalogueItemId == AdditionalServiceId)).Name;

            CommonActions.PageTitle().Should().BeEquivalentTo($"{capabilityName} - {solutionName}".FormatForComparison());
        }

        [Fact]
        public async Task AdditionalServiceEpics_EpicsListDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var epics = await context.CatalogueItemEpics.Include(e => e.Epic).Where(e => e.CatalogueItemId == AdditionalServiceId).ToListAsync();

            PublicBrowsePages.SolutionAction.GetNhsSolutionEpics().Should().BeEquivalentTo(epics.Where(e => !e.Epic.SupplierDefined).Select(e => e.Epic.Name));
            PublicBrowsePages.SolutionAction.GetSupplierSolutionEpics().Should().BeEquivalentTo(epics.Where(e => e.Epic.SupplierDefined).Select(e => e.Epic.Name));
        }
    }
}
