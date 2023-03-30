using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.SupplierDefinedEpics
{
    [Collection(nameof(AdminCollection))]
    public sealed class DeleteEpic : AuthorityTestBase
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
        public void Delete_Inactive_Deletes()
        {
            using var context = GetEndToEndDbContext();
            var epic = new Epic
            {
                Id = "S00999",
                Name = Strings.RandomString(20),
                Description = Strings.RandomString(20),
                CapabilityId = context.Capabilities.First(x => x.CapabilityRef == "C44").Id,
                IsActive = false,
                SupplierDefined = true,
            };

            context.Epics.Add(epic);
            context.SaveChanges();

            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.DeleteEpic),
                new Dictionary<string, string> { { nameof(EpicId), epic.Id.ToString() } });

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.Dashboard))
                .Should()
                .BeTrue();

            context.Epics.AsNoTracking().Any(e => e.Id == epic.Id).Should().BeFalse();
        }
    }
}
