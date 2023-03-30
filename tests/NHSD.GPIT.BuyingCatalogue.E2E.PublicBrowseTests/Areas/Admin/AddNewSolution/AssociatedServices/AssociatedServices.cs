using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.AssociatedServices
{
    [Collection(nameof(AdminCollection))]
    public sealed class AssociatedServices : AuthorityTestBase
    {
        private const string TargetServiceId = "99999-S-999";
        private static readonly CatalogueItemId SolutionId = new(99999, "001");
        private static readonly CatalogueItemId SolutionIdNoAssociatedServices = new(99997, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public AssociatedServices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.AssociatedServices),
                  Parameters)
        {
        }

        [Fact]
        public async Task AssociatedServices_CorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.FirstAsync(s => s.Id == SolutionId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Associated Services - {solutionName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.ActionLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.AssociatedServices.AssociatedServicesObjects.AssociatedServicesTable).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public async Task AssociatedServices_NoServices_TableNotDisplayed()
        {
            var args = new Dictionary<string, string>
            {
                { nameof(SolutionId), SolutionIdNoAssociatedServices.ToString() },
            };

            NavigateToUrl(typeof(AssociatedServicesController), nameof(AssociatedServicesController.AssociatedServices), parameters: args);

            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.FirstAsync(s => s.Id == SolutionIdNoAssociatedServices)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Associated Services - {solutionName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.ActionLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.AssociatedServices.AssociatedServicesObjects.AssociatedServicesTable).Should().BeFalse();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AssociatedServices_ClickGoBackLink_NavigatesToCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task AssociatedServices_Save_Saves_And_NavigatesToCorrectPage()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.Include(c => c.SupplierServiceAssociations).FirstAsync(s => s.Id == SolutionId);
            solution.SupplierServiceAssociations.Clear();
            await context.SaveChangesAsync();

            NavigateToUrl(typeof(AssociatedServicesController), nameof(AssociatedServicesController.AssociatedServices), parameters: Parameters);

            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();

            solution = await context.CatalogueItems.Include(c => c.SupplierServiceAssociations).FirstAsync(s => s.Id == SolutionId);

            var savedRelatedService = solution.SupplierServiceAssociations.First();

            savedRelatedService.AssociatedServiceId.Should().BeEquivalentTo(CatalogueItemId.ParseExact("99999-S-999"));
        }

        [Fact]
        public async Task AssociatedServices_NoServices_Save_Saves_And_NavigatesToCorrectPage()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.CatalogueItems.Include(c => c.SupplierServiceAssociations).FirstAsync(s => s.Id == SolutionId);
            solution.SupplierServiceAssociations.Clear();
            await context.SaveChangesAsync();

            var args = new Dictionary<string, string>
            {
                { nameof(SolutionId), SolutionIdNoAssociatedServices.ToString() },
            };

            NavigateToUrl(typeof(AssociatedServicesController), nameof(AssociatedServicesController.AssociatedServices), parameters: args);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();

            solution = await context.CatalogueItems.Include(c => c.SupplierServiceAssociations).FirstAsync(s => s.Id == SolutionId);

            solution.SupplierServiceAssociations.Should().BeEmpty();
        }

        [Fact]
        public void AssociatedServices_ClickAddAssociatedService_ExpectedResult()
        {
            CommonActions.ClickLinkElement(CommonSelectors.ActionLink);

            CommonActions.PageLoadedCorrectGetIndex(typeof(AssociatedServicesController), nameof(AssociatedServicesController.AddAssociatedService))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AssociatedServices_ClickEditLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Admin.AssociatedServices.AssociatedServicesObjects.EditLink, TargetServiceId);

            CommonActions.PageLoadedCorrectGetIndex(typeof(AssociatedServicesController), nameof(AssociatedServicesController.EditAssociatedService))
                .Should()
                .BeTrue();
        }
    }
}
