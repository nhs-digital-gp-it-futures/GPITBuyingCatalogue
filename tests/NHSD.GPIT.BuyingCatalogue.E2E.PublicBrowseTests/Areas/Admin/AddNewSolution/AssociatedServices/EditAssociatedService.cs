using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class EditAssociatedService : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");
        private static readonly CatalogueItemId AssociatedServiceId = new(99999, "S-999");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AssociatedServiceId), AssociatedServiceId.ToString() },
        };

        public EditAssociatedService(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.EditAssociatedService),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditAssociatedService_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.SingleAsync(s => s.Id == SolutionId)).Name;
            var associatedServiceName = (await context.CatalogueItems.SingleAsync(s => s.Id == AssociatedServiceId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{associatedServiceName} information - {solutionName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.AssociatedServices.AssociatedServices.AddAssociatedServiceContinueButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.AssociatedServices.AssociatedServices.AssociatedServiceDashboardTable).Should().BeTrue();
        }

        [Fact]
        public void EditAssociatedService_ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AssociatedServicesController),
                    nameof(AssociatedServicesController.AssociatedServices))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditAssociatedService_ClickContinueButton_NavigatesToCorrectPage()
        {
            CommonActions.ClickLinkElement(Objects.Admin.AssociatedServices.AssociatedServices.AddAssociatedServiceContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AssociatedServicesController),
                    nameof(AssociatedServicesController.AssociatedServices))
                .Should()
                .BeTrue();
        }
    }
}
