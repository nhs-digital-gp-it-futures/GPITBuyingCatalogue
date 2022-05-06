using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    public sealed class DeleteServiceLevel : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int CancelLinkServiceLevelId = 2;
        private const int ServiceLevelId = 3;

        private static readonly CatalogueItemId SolutionId = new(99998, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(ServiceLevelId), ServiceLevelId.ToString() },
        };

        private static readonly Dictionary<string, string> CancelLinkParameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(ServiceLevelId), CancelLinkServiceLevelId.ToString() },
        };

        public DeleteServiceLevel(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.DeleteServiceLevel),
                  Parameters)
        {
        }

        [Fact]
        public void DeleteServiceLevel_CorrectlyDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ServiceLevelObjects.CancelLink).Should().BeTrue();
        }

        [Fact]
        public void DeleteServiceLevel_ClickCancel_ExpectedResult()
        {
            NavigateToUrl(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.DeleteServiceLevel),
                CancelLinkParameters);

            CommonActions.ClickLinkElement(ServiceLevelObjects.CancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevel))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void DeleteServiceLevel_ClickGoBack_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevel))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task DeleteServiceLevel_ClickDelete_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();

            await using var context = GetEndToEndDbContext();

            var serviceLevels = await context.SlaServiceLevels.SingleOrDefaultAsync(slac => slac.Id == ServiceLevelId);

            serviceLevels.Should().BeNull();
        }
    }
}
