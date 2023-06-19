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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes.Desktop
{
    [Collection(nameof(AdminCollection))]
    public sealed class SupportedOperatingSystems : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public SupportedOperatingSystems(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DesktopBasedController),
                  nameof(DesktopBasedController.OperatingSystems),
                  Parameters)
        {
        }

        [Fact]
        public async Task SupportedOperatingSystems_SaveData()
        {
            var description = TextGenerators.TextInputAddText(Objects.Common.CommonSelectors.Description, 1000);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var solution = await context.Solutions.FirstOrDefaultAsync(s => s.CatalogueItemId == SolutionId);

            var mobileOperatingSystems = solution.ApplicationTypeDetail?.NativeDesktopOperatingSystemsDescription;

            mobileOperatingSystems.Should().NotBeNull();
            mobileOperatingSystems.Should().Be(description);
        }

        [Fact]
        public void SupportedOperatingSystems_ErrorThrownMissingMandatory()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                "Description",
                "Enter supported operating systems information")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void SupportedOperatingSystems_ClickGoBack()
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
