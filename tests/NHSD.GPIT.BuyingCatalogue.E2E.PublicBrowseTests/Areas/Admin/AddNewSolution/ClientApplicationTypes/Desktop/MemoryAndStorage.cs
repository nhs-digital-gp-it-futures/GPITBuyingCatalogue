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
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes.Desktop
{
    [Collection(nameof(AdminCollection))]
    public sealed class MemoryAndStorage : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public MemoryAndStorage(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DesktopBasedController),
                  nameof(DesktopBasedController.MemoryAndStorage),
                  Parameters)
        {
        }

        [Fact]
        public async Task MemoryAndStorage_SaveData()
        {
            var memorySize = CommonActions.SelectRandomDropDownItem(Objects.Admin.EditSolution.ClientApplicationObjects.MinimumMemoryDropDown);

            var storageSize = TextGenerators.TextInputAddText(Objects.Admin.EditSolution.ClientApplicationObjects.StorageSpace, 300);

            var minimumCpu = TextGenerators.TextInputAddText(Objects.Admin.EditSolution.ClientApplicationObjects.ProcessingPower, 300);

            var resolution = CommonActions.SelectRandomDropDownItem(Objects.Admin.EditSolution.ClientApplicationObjects.ResolutionDropdown);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var solution = await context.Solutions.FirstOrDefaultAsync(s => s.CatalogueItemId == SolutionId);

            var memoryAndStorage = solution.GetClientApplication()?.NativeDesktopMemoryAndStorage;

            memoryAndStorage.Should().NotBeNull();
            memoryAndStorage.MinimumMemoryRequirement.Should().Be(memorySize);
            memoryAndStorage.MinimumCpu.Should().Be(minimumCpu);
            memoryAndStorage.StorageRequirementsDescription.Should().Be(storageSize);
            memoryAndStorage.RecommendedResolution.Should().Be(resolution);
        }

        [Fact]
        public void MemoryAndStorage_ErrorThrownMissingMandatory()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            Dictionary<string, string> expectedErrors = new()
            {
                { "SelectedMemorySize", "Select a minimum memory size" },
                { "StorageSpace", "Enter storage space information" },
                { "ProcessingPower", "Enter processing power information" },
            };

            expectedErrors.All(s => CommonActions.ElementShowingCorrectErrorMessage(
                s.Key,
                s.Value))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void MemoryAndStorage_ClickGoBack()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
