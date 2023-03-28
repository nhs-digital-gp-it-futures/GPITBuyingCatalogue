using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    [Collection(nameof(AdminCollection))]
    public sealed class AddSlaType : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AddSlaType(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ServiceLevelAgreementsController),
                  nameof(ServiceLevelAgreementsController.AddServiceLevelAgreement),
                  Parameters)
        {
        }

        [Fact]
        public async Task AddSlaType_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.FirstAsync(ci => ci.Id == SolutionId);

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Catalogue Solution type - {solution.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtonItems).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddSlaType_ClickGoBack()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        [Fact]
        public void AddSlaType_ClickSaveAndContinue_ErrorMessageThrown()
        {
            var solutionId = new CatalogueItemId(99999, "004");

            var parameters = new Dictionary<string, string>()
            {
                { nameof(SolutionId), solutionId.ToString() },
            };

            NavigateToUrl(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddServiceLevelAgreement),
                parameters);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddServiceLevelAgreement))
                .Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed()
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AddSlaType_ClickSave_Valid()
        {
            CommonActions.ClickRadioButtonWithText("Type 1 Catalogue Solution");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var sla = await context.ServiceLevelAgreements.FirstAsync(s => s.SolutionId == SolutionId);

            sla.SlaType.Should().Be(SlaType.Type1);
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var sla = context.ServiceLevelAgreements.FirstOrDefault(s => s.SolutionId == SolutionId);
            if (sla is not null)
            {
                context.ServiceLevelAgreements.Remove(sla);
                context.SaveChanges();
            }
        }
    }
}
