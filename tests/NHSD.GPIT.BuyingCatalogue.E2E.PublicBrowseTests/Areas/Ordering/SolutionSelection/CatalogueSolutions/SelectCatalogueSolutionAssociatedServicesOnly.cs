﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.CatalogueSolutions
{
    public class SelectCatalogueSolutionAssociatedServicesOnly : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const string SolutionName = "E2E With Contact With Single Price";
        private const int OrderId = 90018;

        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public SelectCatalogueSolutionAssociatedServicesOnly(LocalWebApplicationFactory factory)
            : base(factory, typeof(CatalogueSolutionsController), nameof(CatalogueSolutionsController.SelectSolutionAssociatedServicesOnly), Parameters)
        {
        }

        [Fact]
        public void SelectSolutionAssociatedServicesOnly_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Which Catalogue Solution does the service help implement? - Order {CallOffId}".FormatForComparison());
            CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SelectSolution).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SelectSolutionAssociatedServicesOnly_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void SelectSolutionAssociatedServicesOnly_NoSelectionMade_DisplaysError()
        {
            GetSolution().Should().BeNull();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectSolutionAssociatedServicesOnly)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SelectSolutionErrorMessage)
                .Should().BeTrue();
        }

        [Fact]
        public void SelectSolutionAssociatedServicesOnly_SelectSolution_ExpectedResult()
        {
            GetSolution().Should().BeNull();

            CommonActions.ClickRadioButtonWithText(SolutionName);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.SelectAssociatedServices)).Should().BeTrue();

            var solution = GetSolution();

            solution.Should().NotBeNull();
            solution.Name.Should().Be(SolutionName);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            var order = context.Orders.Single(x => x.Id == OrderId);

            order.SolutionId = null;

            context.SaveChanges();
        }

        private CatalogueItem GetSolution()
        {
            return GetEndToEndDbContext().Orders
                .Include(x => x.Solution)
                .Single(x => x.Id == OrderId)
                .Solution;
        }
    }
}
