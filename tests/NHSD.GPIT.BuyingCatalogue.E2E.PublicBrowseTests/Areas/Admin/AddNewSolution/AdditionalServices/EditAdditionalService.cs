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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AdditionalServices
{
    public sealed class EditAdditionalService : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99999, "001A999");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
        };

        public EditAdditionalService(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.EditAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditAdditionalService_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.SingleAsync(s => s.Id == SolutionId)).Name;
            var additionalServiceName = (await context.CatalogueItems.SingleAsync(s => s.Id == AdditionalServiceId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"{additionalServiceName} information - {solutionName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.ContinueButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.AdditionalServices.AdditionalServices.AdditionalServicesTableDashboard).Should().BeTrue();
        }

        [Fact]
        public void EditAdditionalService_ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditAdditionalService_ClickContinueButton_NavigatesToCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.ContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AdditionalServicesController),
                    nameof(AdditionalServicesController.Index))
                .Should()
                .BeTrue();
        }
    }
}
