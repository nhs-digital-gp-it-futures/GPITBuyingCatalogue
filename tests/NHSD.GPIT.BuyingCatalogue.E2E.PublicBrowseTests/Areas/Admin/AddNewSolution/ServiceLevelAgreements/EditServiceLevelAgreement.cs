using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ServiceLevelAgreements
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditServiceLevelAgreement : AuthorityTestBase
    {
        private static readonly CatalogueItemId SolutionId = new(99998, "001");

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
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.FirstAsync(ci => ci.Id == SolutionId);

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
        public void EditServiceLevelAgreement_ClickGoBack()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        [Fact]
        public void EditServiceLevelAgreement_ClickContinue()
        {
            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }
    }
}
