using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    [Collection(nameof(AdminCollection))]
    public sealed class DeleteServiceAvailabilityTimes : AuthorityTestBase
    {
        private const int CancelLinkServiceAvailabilityTimesId = 2;
        private const int ServiceAvailabilityTimesId = 3;
        private static readonly CatalogueItemId SolutionId = new(99998, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(ServiceAvailabilityTimesId), ServiceAvailabilityTimesId.ToString() },
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
        public void DeleteServiceAvailabilityTimes_ClickDelete_ExpectedResult()
        {
            using var context = GetEndToEndDbContext();
            var serviceLevelAgreement = context.ServiceLevelAgreements.Include(p => p.ServiceHours).First(p => p.SolutionId == SolutionId);
            var serviceLevelHours = new ServiceAvailabilityTimes()
            {
                ApplicableDays = "Applicable Days 02",
                Category = "Support Type 02",
                TimeFrom = DateTime.UtcNow.AddHours(-5),
                TimeUntil = DateTime.UtcNow,
            };

            serviceLevelAgreement.ServiceHours.Add(serviceLevelHours);
            context.SaveChanges();

            var parameters = new Dictionary<string, string>
            {
                { nameof(SolutionId), SolutionId.ToString() },
                { nameof(ServiceAvailabilityTimesId), serviceLevelHours.Id.ToString() },
            };

            NavigateToUrl(
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.DeleteServiceAvailabilityTimes),
                  parameters);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }
    }
}
