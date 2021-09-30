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
    public sealed class AddAssociatedService : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddAssociatedService(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.AddAssociatedService),
                  Parameters)
        {
        }

        [Fact]
        public async Task AddAssociatedService_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.SingleAsync(s => s.Id == SolutionId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Associated Service information - {solutionName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.ContinueButton).Should().BeTrue();
        }

        [Fact]
        public void AddAssociatedService_ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AssociatedServicesController),
                    nameof(AssociatedServicesController.AssociatedServices))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddAssociatedService_ClickContinueButton_NavigatesToCorrectPage()
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
