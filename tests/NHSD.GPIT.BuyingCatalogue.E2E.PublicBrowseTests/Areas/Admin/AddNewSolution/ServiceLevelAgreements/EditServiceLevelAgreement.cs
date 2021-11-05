using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    public sealed class EditServiceLevelAgreement : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public EditServiceLevelAgreement(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditServiceLevelAgreement_DisplaysCorrectly()
        {
            await AddSlaToSolution();

            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == SolutionId);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Service Level Agreement - {solution.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink)
                .Should().BeTrue();

            CommonActions.ElementIsDisplayed(EditServiceLevelAgreementsObjects.CatalogueSolutionType)
                .Should().BeTrue();

            CommonActions.NumberOfElementsDisplayed(CommonSelectors.ActionLink)
                .Should().Be(4);

            CommonActions.ElementIsDisplayed(EditServiceLevelAgreementsObjects.ContactDetailsTable)
                .Should().BeTrue();

            CommonActions.ElementIsDisplayed(EditServiceLevelAgreementsObjects.ServiceHoursTable)
                .Should().BeTrue();

            CommonActions.ElementIsDisplayed(EditServiceLevelAgreementsObjects.ServiceLevelsTable)
                .Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.ContinueButton)
                .Should().BeTrue();
        }

        [Fact]
        public async Task EditServiceLevelAgreement_ClickGoBack()
        {
            await AddSlaToSolution();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        [Fact]
        public async Task EditServiceLevelAgreement_ClickContinue()
        {
            await AddSlaToSolution();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        private async Task AddSlaToSolution()
        {
            await using var context = GetEndToEndDbContext();
            var sla = new EntityFramework.Catalogue.Models.ServiceLevelAgreements
            {
                SolutionId = SolutionId,
                SlaType = SlaType.Type2,
            };

            var solution = await context.Solutions
                .Include(s => s.ServiceLevelAgreement)
                .SingleAsync(s => s.CatalogueItemId == SolutionId);
            solution.ServiceLevelAgreement = sla;
            await context.SaveChangesAsAsync(UserSeedData.BobId);
            Driver.Navigate().Refresh();
        }
    }
}
