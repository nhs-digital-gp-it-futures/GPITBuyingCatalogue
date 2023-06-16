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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ApplicationTypes.Desktop
{
    [Collection(nameof(AdminCollection))]
    public sealed class ThirdPartyComponents : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ThirdPartyComponents(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DesktopBasedController),
                  nameof(DesktopBasedController.ThirdPartyComponents),
                  Parameters)
        {
        }

        [Fact]
        public async Task ThirdPartyComponents_SaveData()
        {
            var thirdPartyComponentDescription = TextGenerators.TextInputAddText(Objects.Admin.EditSolution.ApplicationTypeObjects.ThirdPartyComponents, 500);

            var deviceCapabilities = TextGenerators.TextInputAddText(Objects.Admin.EditSolution.ApplicationTypeObjects.DeviceCapabilities, 500);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var solution = await context.Solutions.FirstOrDefaultAsync(s => s.CatalogueItemId == SolutionId);

            var thirdPartyComponents = JsonDeserializer.Deserialize<ServiceContracts.Solutions.ApplicationTypes>(solution.ApplicationType)
                ?.NativeDesktopThirdParty;

            thirdPartyComponents.Should().NotBeNull();

            thirdPartyComponents.ThirdPartyComponents.Should().Be(thirdPartyComponentDescription);
            thirdPartyComponents.DeviceCapabilities.Should().Be(deviceCapabilities);
        }

        [Fact]
        public void ThirdPartyComponents_ClickGoBack()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();
        }

        public void Dispose()
        {
            ClearApplicationType(SolutionId);
        }
    }
}
