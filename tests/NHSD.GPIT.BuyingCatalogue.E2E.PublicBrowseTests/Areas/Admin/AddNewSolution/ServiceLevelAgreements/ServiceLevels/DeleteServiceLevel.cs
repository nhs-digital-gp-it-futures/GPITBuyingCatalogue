using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    [Collection(nameof(AdminCollection))]
    public sealed class DeleteServiceLevel : AuthorityTestBase
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
            var serviceLevel = await AddServiceLevel();

            NavigateToUrl(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.DeleteServiceLevel),
                new Dictionary<string, string>
                {
                    { nameof(SolutionId), SolutionId.ToString() },
                    { nameof(ServiceLevelId), serviceLevel.Id.ToString() },
                });

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();

            var context = GetEndToEndDbContext();

            context.SlaServiceLevels.Count(x => x.Id == serviceLevel.Id).Should().Be(0);
        }

        private async Task<SlaServiceLevel> AddServiceLevel()
        {
            await using var context = GetEndToEndDbContext();

            var serviceLevel = new SlaServiceLevel
            {
                SolutionId = SolutionId,
                TypeOfService = Strings.RandomString(10),
                ServiceLevel = Strings.RandomString(10),
                HowMeasured = Strings.RandomString(10),
                ServiceCredits = true,
            };

            context.SlaServiceLevels.Add(serviceLevel);
            await context.SaveChangesAsync();

            return serviceLevel;
        }
    }
}
