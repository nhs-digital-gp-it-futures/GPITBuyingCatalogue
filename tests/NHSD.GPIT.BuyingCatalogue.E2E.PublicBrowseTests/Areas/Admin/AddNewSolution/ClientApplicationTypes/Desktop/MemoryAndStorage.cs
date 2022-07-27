using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes.Desktop
{
    public sealed class MemoryAndStorage : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
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

            var solution = await context.Solutions.SingleOrDefaultAsync(s => s.CatalogueItemId == SolutionId);

            var memoryAndStorage = JsonDeserializer.Deserialize<ServiceContracts.Solutions.ClientApplication>(solution.ClientApplication)
                ?.NativeDesktopMemoryAndStorage;

            memoryAndStorage.Should().NotBeNull();
            memoryAndStorage.MinimumMemoryRequirement.Should().Be(memorySize);
            memoryAndStorage.MinimumCpu.Should().Be(minimumCpu);
            memoryAndStorage.StorageRequirementsDescription.Should().Be(storageSize);
            memoryAndStorage.RecommendedResolution.Should().Be(resolution);
        }

        [Fact]
        public void MemoryAndStorage_MissingMandatory_ErrorsThrown()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.MemoryAndStorage))
                .Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(DesktopApplicationObjects.MemorySizeError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DesktopApplicationObjects.StorageSpaceError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DesktopApplicationObjects.ProcessingPowerError).Should().BeTrue();
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
