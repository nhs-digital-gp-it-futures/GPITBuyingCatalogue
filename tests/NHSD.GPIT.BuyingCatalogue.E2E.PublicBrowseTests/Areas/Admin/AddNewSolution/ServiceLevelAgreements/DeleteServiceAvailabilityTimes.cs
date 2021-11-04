using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    public sealed class DeleteServiceAvailabilityTimes : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int CancelLinkServiceAvailabilityTimesId = 2;
        private const int ServiceAvailabilityTimesId = 3;
        private static readonly CatalogueItemId SolutionId = new(99998, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(ServiceAvailabilityTimesId), ServiceAvailabilityTimesId.ToString() },
        };

        private static readonly Dictionary<string, string> CancelLinkParameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(ServiceAvailabilityTimesId), CancelLinkServiceAvailabilityTimesId.ToString() },
        };

        public DeleteServiceAvailabilityTimes(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.DeleteServiceAvailabilityTimes),
                  Parameters)
        {
        }

        [Fact]
        public void DeleteServiceAvailabilityTimes_CorrectlyDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceAvailabilityTimesObjects.CancelLink).Should().BeTrue();
        }

        [Fact]
        public void DeleteServiceAvailabilityTimes_ClickCancel_ExpectedResult()
        {
            NavigateToUrl(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.DeleteServiceAvailabilityTimes),
                CancelLinkParameters);

            CommonActions.ClickLinkElement(ServiceAvailabilityTimesObjects.CancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceAvailabilityTimes))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void DeleteServiceAvailabilityTimes_ClickGoBack_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceAvailabilityTimes))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task DeleteServiceAvailabilityTimes_ClickDelete_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();

            await using var context = GetEndToEndDbContext();

            var serviceAvailabilityTimes = await context.ServiceAvailabilityTimes.SingleOrDefaultAsync(slac => slac.Id == ServiceAvailabilityTimesId);

            serviceAvailabilityTimes.Should().BeNull();
        }
    }
}
