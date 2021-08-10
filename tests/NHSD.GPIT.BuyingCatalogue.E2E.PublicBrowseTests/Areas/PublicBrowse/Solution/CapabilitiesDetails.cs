﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class CapabilitiesDetails : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public CapabilitiesDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SolutionDetailsController),
                  nameof(SolutionDetailsController.Capabilities),
                  Parameters)
        {
        }

        [Fact]
        public async Task CapabilitiesDetails_VerifyCapabilities()
        {
            await using var context = GetEndToEndDbContext();
            var capabilitiesInfo =
                (await context.CatalogueItems
                    .Include(ci => ci.CatalogueItemCapabilities)
                        .ThenInclude(s => s.Capability)
                    .SingleAsync(s => s.Id == new CatalogueItemId(99999, "001")))
                .CatalogueItemCapabilities
                .Select(s => s.Capability);

            var capabilitiesList = PublicBrowsePages.SolutionAction.GetCapabilitiesContent().ToArray()[0];

            var capabilitiesTitle = capabilitiesInfo.Select(c => c.Name.Trim());
            foreach (var name in capabilitiesTitle)
            {
                capabilitiesList.Should().Contain(name);
            }

            var capabilitiesDescription = capabilitiesInfo.Select(b => b.Description.Trim());
            foreach (var description in capabilitiesDescription)
            {
                capabilitiesList.Should().Contain(description);
            }
        }

        [Fact]
        public async Task CapabilitiesDetails_CheckEpics_NhsDefinedSolutionEpics()
        {
            PublicBrowsePages.SolutionAction.ClickEpics();
            var nhsEpicsList = PublicBrowsePages.SolutionAction.GetNhsSolutionEpics().ToArray();

            await using var context = GetEndToEndDbContext();
            var nhsEpicsInfo = (await context.CatalogueItems.Include(s => s.CatalogueItemEpics).ThenInclude(s => s.Epic).SingleAsync(s => s.Id == new CatalogueItemId(99999, "001"))).CatalogueItemEpics.Select(s => s.Epic);

            nhsEpicsList.Should().BeEquivalentTo(nhsEpicsInfo.Where(e => !e.SupplierDefined).Select(c => c.Name));
        }

        [Fact]
        public async Task CapabilitiesDetails_CheckEpics_SupplierDefinedSolutionEpics()
        {
            PublicBrowsePages.SolutionAction.ClickEpics();
            var supplierEpicsList = PublicBrowsePages.SolutionAction.GetSupplierSolutionEpics();

            await using var context = GetEndToEndDbContext();
            var supplierEpicsInfo = (await context.CatalogueItems.Include(s => s.CatalogueItemEpics).ThenInclude(s => s.Epic).SingleAsync(s => s.Id == new CatalogueItemId(99999, "001"))).CatalogueItemEpics.Select(s => s.Epic);

            supplierEpicsList.Should().BeEquivalentTo(supplierEpicsInfo.Where(e => e.SupplierDefined).Select(c => c.Name));
        }
    }
}
