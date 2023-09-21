using System;
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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.CatalogueSolutions
{
    [Collection(nameof(OrderingCollection))]
    public class SelectCatalogueSolution : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const string SolutionName = "E2E With Contact With Single Price";
        private const string AdditionalServiceName = "E2E No Contact Single Price Additional Service";
        private const int OrderId = 90004;

        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public SelectCatalogueSolution(LocalWebApplicationFactory factory)
            : base(factory, typeof(CatalogueSolutionsController), nameof(CatalogueSolutionsController.SelectSolution), Parameters)
        {
        }

        [Fact]
        public void SelectCatalogueSolution_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Catalogue Solutions - E2E Test Supplier With Contact".FormatForComparison());
            CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SelectSolution).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SelectCatalogueSolution_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void SelectCatalogueSolution_NoSelectionMade_DisplaysError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectSolution)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CatalogueSolutionObjects.SelectSolutionErrorMessage,
                $"Error:{SelectSolutionModelValidator.NoSelectionMadeErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void SelectCatalogueSolution_SelectSolution_ExpectedResult()
        {
            GetOrderItems().Should().BeEmpty();

            CommonActions.ClickRadioButtonWithText(SolutionName);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.SelectServiceRecipients)).Should().BeTrue();

            var orderItems = GetOrderItems();

            orderItems.Count.Should().Be(1);
            orderItems[0].CatalogueItem.CatalogueItemType.Should().Be(CatalogueItemType.Solution);
            orderItems[0].CatalogueItem.Name.Should().Be(SolutionName);
        }

        [Fact]
        public void SelectCatalogueSolution_SelectSolutionAndAdditionalService_ExpectedResult()
        {
            GetOrderItems().Should().BeEmpty();

            CommonActions.ClickRadioButtonWithText(SolutionName);
            CommonActions.ClickCheckboxByLabel(AdditionalServiceName);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceRecipientsController),
                nameof(ServiceRecipientsController.SelectServiceRecipients)).Should().BeTrue();

            var orderItems = GetOrderItems();

            orderItems.Count.Should().Be(2);

            orderItems.Should().Contain(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution
                && x.CatalogueItem.Name == SolutionName);

            orderItems.Should().Contain(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                && x.CatalogueItem.Name == AdditionalServiceName);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            foreach (var item in GetOrderItems())
            {
                context.OrderItems.Remove(item);
            }

            context.SaveChanges();
        }

        private List<OrderItem> GetOrderItems()
        {
            var context = GetEndToEndDbContext();

            return context.OrderItems
                .Include(x => x.CatalogueItem)
                .Where(x => x.OrderId == OrderId)
                .ToList();
        }
    }
}
