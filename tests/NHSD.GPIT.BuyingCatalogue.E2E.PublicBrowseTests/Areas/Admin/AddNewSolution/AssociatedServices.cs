﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class AssociatedServices : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "001");

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
            var solutionName = (await context.CatalogueItems.SingleAsync(s => s.Id == SolutionId)).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Associated Services - {solutionName}".FormatForComparison());

            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.ActionLink).Should().BeTrue();

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
            var solution = await context.CatalogueItems.Include(c => c.SupplierServiceAssociations).SingleAsync(s => s.Id == SolutionId);
            solution.SupplierServiceAssociations.Clear();
            await context.SaveChangesAsync();

            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();

            solution = await context.CatalogueItems.Include(c => c.SupplierServiceAssociations).SingleAsync(s => s.Id == SolutionId);

            var savedRelatedService = solution.SupplierServiceAssociations.Single();

            savedRelatedService.AssociatedServiceId.Should().BeEquivalentTo(CatalogueItemId.ParseExact("99999--S-999"));
        }
    }
}
