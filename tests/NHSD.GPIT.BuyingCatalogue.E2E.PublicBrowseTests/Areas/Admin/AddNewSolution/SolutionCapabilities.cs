using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    [Collection(nameof(AdminCollection))]
    public sealed class SolutionCapabilities : AuthorityTestBase
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public SolutionCapabilities(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.EditCapabilities),
                  Parameters)
        {
        }

        [Fact]
        public async Task AllSectionsDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var capabilityCategories = await context.CapabilityCategories.Include(cc => cc.Capabilities).Where(cc => cc.Capabilities.Any()).ToListAsync();
            var solution = await context.CatalogueItems.Include(i => i.Supplier).FirstAsync(s => s.Id == SolutionId);
            var solutionName = solution.Name;

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Capabilities and Epics - {solutionName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1)
                .Should()
                .BeTrue();

            capabilityCategories.ForEach(cc => CommonActions.ElementIsDisplayed(By.XPath($"//h2[text()='{cc.Name}']")).Should().BeTrue());
        }

        [Fact]
        public async Task Submit_NoCapabilitiesSelected_AddsSummaryError()
        {
            await using var context = GetEndToEndDbContext();
            var catalogueItemCapabilities = await context.CatalogueItemCapabilities.AsNoTracking().Where(c => c.CatalogueItemId == SolutionId).ToListAsync();
            context.CatalogueItemCapabilities.RemoveRange(catalogueItemCapabilities);
            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryLinksExist()
                .Should()
                .BeTrue();

            context.CatalogueItemCapabilities.AddRange(catalogueItemCapabilities);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task Submit_CapabilitySelected_SavesCapability()
        {
            await using var context = GetEndToEndDbContext();
            var capability = await context.Capabilities.Include(c => c.Epics).FirstAsync();
            var mustEpic = capability.CapabilityEpics.First(e => e.CompliancyLevel == CompliancyLevel.Must).Epic;

            CommonActions.ClickCheckboxByLabel($"({capability.CapabilityRef}) {capability.Name}");
            CommonActions.ClickCheckboxByLabel($"({mustEpic.Id}) {mustEpic.Name}");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();

            var catalogueItemCapability = await context.CatalogueItemCapabilities.FirstOrDefaultAsync(c => c.CatalogueItemId == SolutionId && c.CapabilityId == capability.Id);

            catalogueItemCapability.Should().NotBeNull();
        }

        [Fact]
        public void Submit_CapabilitiesSelected_PreselectsCapabilities()
        {
            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();

            NavigateToUrl(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditCapabilities),
                Parameters);

            CommonActions.GetNumberOfSelectedCheckBoxes().Should().BeGreaterThan(0);
        }

        [Fact]
        public void ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();
        }
    }
}
