using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.SupplierDefinedEpics
{
    public sealed class DeleteEpic : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string EpicId = "S00007";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(EpicId), EpicId },
        };

        public DeleteEpic(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SupplierDefinedEpicsController),
                  nameof(SupplierDefinedEpicsController.DeleteEpic),
                  Parameters)
        {
        }

        [Fact]
        public void Delete_AllSectionsDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var epic = context.Epics.AsNoTracking().First(e => e.Id == EpicId);

            CommonActions.PageTitle().Should().Be($"Delete {epic.Name}?".FormatForComparison());
            CommonActions.LedeText().Should().Be("Confirm you want to delete this supplier defined Epic.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.CancelLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void Delete_Deletes()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.Dashboard))
                .Should()
                .BeTrue();

            using var context = GetEndToEndDbContext();
            context.Epics.AsNoTracking().Any(e => e.Id == EpicId).Should().BeFalse();
        }

        [Fact]
        public void Delete_ActiveNoReferences_RedirectsToEditPage()
        {
            const string epicId = "S00005";
            var parameters = new Dictionary<string, string>
            {
                { nameof(EpicId), epicId },
            };

            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.DeleteEpic),
                parameters);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditEpic))
                .Should().BeTrue();

            using var context = GetEndToEndDbContext();
            context.Epics.Any(e => e.Id == epicId).Should().BeTrue();
        }

        [Fact]
        public void Delete_ActiveWithReferences_RedirectsToDashboard()
        {
            const string epicId = "S00006";
            var parameters = new Dictionary<string, string>
            {
                { nameof(EpicId), epicId },
            };

            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.DeleteEpic),
                parameters);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditEpic))
                .Should().BeTrue();

            using var context = GetEndToEndDbContext();
            context.Epics.Any(e => e.Id == epicId).Should().BeTrue();
        }
    }
}
