using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes.MobileOrTablet
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
                  typeof(MobileTabletBasedController),
                  nameof(MobileTabletBasedController.HardwareRequirements),
                  Parameters)
        {
        }

        [Fact]
        public async Task HardwareRequirements_SavePage()
        {
            var expectedRequirements = TextGenerators.TextInputAddText(Objects.Common.CommonSelectors.Description, 500);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.MobileTablet)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);

            var hardwareRequirements = solution.ApplicationTypeDetail?.NativeMobileHardwareRequirements;

            hardwareRequirements.Should().NotBeNull();
            hardwareRequirements.Should().Be(expectedRequirements);
        }

        [Fact]
        public void HardwareRequirements_ClickGoBack()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.MobileTablet))
                .Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
