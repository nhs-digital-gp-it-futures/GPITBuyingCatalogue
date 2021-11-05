using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
    public sealed class EditSlaType : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public EditSlaType(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.EditServiceLevelAgreementType),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditSlaType_CorrectlyDisplayed()
        {
            await AddSlaToSolution();
            Driver.Navigate().Refresh();

            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == SolutionId);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Type of Catalogue Solution - {solution.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtonItems).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task EditSlaType_ClickGoBack()
        {
            await AddSlaToSolution();
            Driver.Navigate().Refresh();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should().BeTrue();
        }

        [Fact]
        public async Task EditSlaType_ClickSave_Valid()
        {
            await AddSlaToSolution();
            Driver.Navigate().Refresh();

            CommonActions.ClickRadioButtonWithText("Type 1 Catalogue Solution");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var sla = await context.ServiceLevelAgreements.SingleAsync(s => s.SolutionId == SolutionId);

            sla.SlaType.Should().Be(SlaType.Type1);
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
        }
    }
}
