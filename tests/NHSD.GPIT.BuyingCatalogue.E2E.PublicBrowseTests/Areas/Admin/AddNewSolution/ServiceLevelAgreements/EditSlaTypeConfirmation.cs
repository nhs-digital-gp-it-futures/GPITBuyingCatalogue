using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    public sealed class EditSlaTypeConfirmation : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public EditSlaTypeConfirmation(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.EditServiceLevelAgreementTypeConfirmation),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditSlaTypeConfirmation_CorrectlyDisplayed()
        {
            await AddSlaToSolution();
            Driver.Navigate().Refresh();

            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.SingleAsync(ci => ci.Id == SolutionId);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Catalogue Solution type - {solution.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task EditSlaTypeConfirmation_ClickGoBack()
        {
            await AddSlaToSolution();
            Driver.Navigate().Refresh();

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreementType))
                .Should().BeTrue();
        }

        [Fact]
        public void EditSlaTypeConfirmation_ClickCancelLink()
        {
            CommonActions.ClickLinkElement(EditSlaTypeConfirmationObjects.EditSleTypeConfirmationCancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreementType))
                .Should().BeTrue();
        }

        [Fact]
        public async Task EditSlaTypeConfirmation_ClickSave_Valid()
        {
            await AddSlaToSolution();
            Driver.Navigate().Refresh();

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
            await context.SaveChangesAsync();
        }
    }
}
