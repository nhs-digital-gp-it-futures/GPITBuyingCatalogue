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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes.BrowserBased
{
    [Collection(nameof(AdminCollection))]
    public sealed class ConnectivityAndResolution : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public ConnectivityAndResolution(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(BrowserBasedController),
                  nameof(BrowserBasedController.ConnectivityAndResolution),
                  Parameters)
        {
        }

        [Fact]
        public async Task ConnectivityAndResolution_SavePage()
        {
            var connectionSpeed = CommonActions.SelectRandomDropDownItem(Objects.Admin.AddSolutionObjects.ConnectivityDropdown);

            var resolution = CommonActions.SelectRandomDropDownItem(Objects.Admin.AddSolutionObjects.ResolutionDropdown);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);

            var clientApplication = solution.GetClientApplication();

            clientApplication.Should().NotBeNull();

            clientApplication.MinimumConnectionSpeed.Should().Be(connectionSpeed);
            clientApplication.MinimumDesktopResolution.Should().Be(resolution);
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
