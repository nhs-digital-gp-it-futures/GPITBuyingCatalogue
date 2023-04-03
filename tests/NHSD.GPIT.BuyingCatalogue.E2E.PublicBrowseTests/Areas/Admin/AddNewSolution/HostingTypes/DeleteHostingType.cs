using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using CommonSelectors = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.CommonSelectors;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.HostingTypes
{
    [Collection(nameof(AdminCollection))]
    public sealed class DeleteHostingType : AuthorityTestBase
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");
        private static readonly ServiceContracts.Solutions.HostingType HostingType = ServiceContracts.Solutions.HostingType.Hybrid;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(HostingType), HostingType.ToString() },
        };

        public DeleteHostingType(LocalWebApplicationFactory factory)
           : base(
                 factory,
                 typeof(HostingTypesController),
                 nameof(HostingTypesController.DeleteHostingType),
                 Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(CommonSelectors.Header1)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(HostingTypesObjects.DeleteHostingTypeCancelLink)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HostingTypesController),
                    nameof(HostingTypesController.Hybrid))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ClickCancelLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(HostingTypesObjects.DeleteHostingTypeCancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HostingTypesController),
                    nameof(HostingTypesController.Hybrid))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task ClickDelete_HostingModelDeleted()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HostingTypesController),
                    nameof(HostingTypesController.HostingType))
                .Should()
                .BeTrue();

            await using var context = GetEndToEndDbContext();

            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);
            var hostingModel = solution.Hosting.HybridHostingType;

            hostingModel.Summary.Should().BeNull();
            hostingModel.Link.Should().BeNull();
            hostingModel.HostingModel.Should().BeNull();
        }
    }
}
