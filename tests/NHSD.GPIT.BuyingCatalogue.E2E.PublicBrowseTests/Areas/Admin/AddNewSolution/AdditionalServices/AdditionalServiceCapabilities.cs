﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AdditionalServices
{
    public sealed class AdditionalServiceCapabilities : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99999, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
        };

        public AdditionalServiceCapabilities(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.EditCapabilities),
                  Parameters)
        {
        }

        [Fact]
        public async Task AllSectionsDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var capabilityCategories = await context.CapabilityCategories.ToListAsync();
            var additionalService = await context.CatalogueItems.Include(i => i.Supplier).SingleAsync(s => s.Id == AdditionalServiceId);
            var additionalServiceName = additionalService.Name;
            var solutionName = additionalService.Name;

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{additionalServiceName} Capabilities - {solutionName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1)
                .Should()
                .BeTrue();

            capabilityCategories.ForEach(cc => CommonActions.ElementIsDisplayed(By.XPath($"//h2[text()='{cc.Name}']")).Should().BeTrue());
        }

        [Fact]
        public async Task Submit_NoCapabilitiesSelected_AddsSummaryError()
        {
            await using var context = GetEndToEndDbContext();
            var catalogueItemCapabilities = await context.CatalogueItemCapabilities.Where(c => c.CatalogueItemId == AdditionalServiceId).ToListAsync();
            context.CatalogueItemCapabilities.RemoveRange(catalogueItemCapabilities);
            await context.SaveChangesAsAsync(UserSeedData.BobId);

            Driver.Navigate().Refresh();

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            context.CatalogueItemCapabilities.AddRange(catalogueItemCapabilities);
            await context.SaveChangesAsAsync(UserSeedData.BobId);
        }

        [Fact]
        public async Task Submit_CapabilitySelected_SavesCapability()
        {
            await using var context = GetEndToEndDbContext();
            var capability = await context.Capabilities.FirstAsync();

            CommonActions.ClickCheckboxByLabel(capability.Name);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();

            var catalogueItemCapability = await context.CatalogueItemCapabilities.SingleOrDefaultAsync(c => c.CatalogueItemId == AdditionalServiceId && c.CapabilityId == capability.Id);

            catalogueItemCapability.Should().NotBeNull();
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();
        }
    }
}
