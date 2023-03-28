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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.CatalogueSolutions
{
    [Collection(nameof(OrderingCollection))]
    public class ConfirmCatalogueSolutionChangesAssociatedServicesOnly : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "IB-QWO";
        private const int OrderId = 90016;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly CatalogueItemId ExistingServiceId = new(99998, "S-997");
        private static readonly CatalogueItemId ExistingSolutionId = new(99998, "001");
        private static readonly CatalogueItemId NewSolutionId = new(99998, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        private static readonly Dictionary<string, string> QueryParameters = new()
        {
            { "CatalogueItemId", $"{NewSolutionId}" },
        };

        public ConfirmCatalogueSolutionChangesAssociatedServicesOnly(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ConfirmSolutionChangesAssociatedServicesOnly),
                Parameters,
                queryParameters: QueryParameters)
        {
        }

        private OrderItem ExistingService => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == ExistingServiceId);

        [Fact]
        public void ConfirmCatalogueSolutionChanges_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Are you sure you want to change your Catalogue Solution? - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(ConfirmServiceChangesObjects.RemovedItems).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ConfirmServiceChangesObjects.AddedItems).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void ConfirmCatalogueSolutionChanges_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditSolutionAssociatedServicesOnly)).Should().BeTrue();

            GetSolution().Id.Should().Be(ExistingSolutionId);
            ExistingService.Should().NotBeNull();
        }

        [Fact]
        public void ConfirmCatalogueSolutionChanges_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ConfirmSolutionChangesAssociatedServicesOnly)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                ConfirmServiceChangesObjects.ConfirmChangesError,
                $"Error:{ConfirmServiceChangesModelValidator.ErrorMessage}").Should().BeTrue();

            GetSolution().Id.Should().Be(ExistingSolutionId);
            ExistingService.Should().NotBeNull();
        }

        [Fact]
        public void ConfirmCatalogueSolutionChanges_ClickYes_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Yes, I want to confirm changes to my Catalogue Solution");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.SelectAssociatedServices)).Should().BeTrue();

            GetSolution().Id.Should().Be(NewSolutionId);
            ExistingService.Should().BeNull();
        }

        [Fact]
        public void ConfirmCatalogueSolutionChanges_ClickNo_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("No, I want to keep my current Catalogue Solution");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();

            GetSolution().Id.Should().Be(ExistingSolutionId);
            ExistingService.Should().NotBeNull();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            context.Orders.First(x => x.Id == OrderId).SolutionId = ExistingSolutionId;
            context.OrderItems.RemoveRange(context.OrderItems.Where(x => x.OrderId == OrderId));
            context.OrderItems.Add(new OrderItem
            {
                OrderId = OrderId,
                Created = DateTime.Now,
                CatalogueItem = context.CatalogueItems.First(x => x.Id == ExistingServiceId),
            });

            context.SaveChanges();
        }

        private CatalogueItem GetSolution()
        {
            return GetEndToEndDbContext().Orders
                .Include(x => x.Solution)
                .First(x => x.Id == OrderId)
                .Solution;
        }
    }
}
