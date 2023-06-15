using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ApplicationTypes.BrowserBased
{
    [Collection(nameof(AdminCollection))]
    public sealed class HardwareRequirements : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public HardwareRequirements(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(BrowserBasedController),
                  nameof(BrowserBasedController.HardwareRequirements),
                  Parameters)
        {
        }

        [Fact]
        public async Task HardwareRequirements_SavePage()
        {
            var expectedRequirements = TextGenerators.TextInputAddText(Objects.Common.CommonSelectors.Description, 500);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);

            var hardwareRequirements = JsonDeserializer.Deserialize<ServiceContracts.Solutions.ClientApplication>(solution.ApplicationType)
                ?.HardwareRequirements;

            hardwareRequirements.Should().NotBeNull();
            hardwareRequirements.Should().Be(expectedRequirements);
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
