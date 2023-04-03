using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.CatalogueSolutions
{
    [Collection(nameof(OrderingCollection))]
    public class ConfirmCatalogueSolutionWithServicesChanges : BuyerTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 91012;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly CatalogueItemId ExistingSolutionId = new(99998, "001");
        private static readonly CatalogueItemId ExistingAdditionalServiceId = new(99998, "001A99");
        private static readonly CatalogueItemId ExistingAssociatedServiceId = new(99998, "S-999");
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

        public ConfirmCatalogueSolutionWithServicesChanges(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ConfirmSolutionChanges),
                Parameters,
                queryParameters: QueryParameters)
        {
        }

        private OrderItem ExistingSolution => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == ExistingSolutionId);

        private OrderItem ExistingAdditionalService => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == ExistingAdditionalServiceId);

        private OrderItem ExistingAssociatedService => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == ExistingAssociatedServiceId);

        private OrderItem NewSolution => GetEndToEndDbContext().OrderItems
            .FirstOrDefault(x => x.OrderId == OrderId && x.CatalogueItemId == NewSolutionId);

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
                nameof(CatalogueSolutionsController.EditSolution)).Should().BeTrue();

            ExistingSolution.Should().NotBeNull();
            ExistingAdditionalService.Should().NotBeNull();
            ExistingAssociatedService.Should().NotBeNull();
            NewSolution.Should().BeNull();
        }

        [Fact]
        public void ConfirmCatalogueSolutionChanges_ClickSave_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ConfirmSolutionChanges)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                ConfirmServiceChangesObjects.ConfirmChangesError,
                $"Error:{ConfirmServiceChangesModelValidator.ErrorMessage}").Should().BeTrue();

            ExistingSolution.Should().NotBeNull();
            ExistingAdditionalService.Should().NotBeNull();
            ExistingAssociatedService.Should().NotBeNull();
            NewSolution.Should().BeNull();
        }

        [Fact]
        public void ConfirmCatalogueSolutionChanges_ClickYes_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Yes, I want to confirm changes to my Catalogue Solution");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectAdditionalServices)).Should().BeTrue();

            ExistingSolution.Should().BeNull();
            ExistingAdditionalService.Should().BeNull();
            ExistingAssociatedService.Should().BeNull();
            NewSolution.Should().NotBeNull();
        }

        [Fact]
        public void ConfirmCatalogueSolutionChanges_ClickNo_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("No, I want to keep my current Catalogue Solution");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();

            ExistingSolution.Should().NotBeNull();
            ExistingAdditionalService.Should().NotBeNull();
            ExistingAssociatedService.Should().NotBeNull();
            NewSolution.Should().BeNull();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            var orderItems = context.OrderItems
                .Include(x => x.CatalogueItem)
                .Where(x => x.OrderId == OrderId);

            context.OrderItems.RemoveRange(orderItems);

            foreach (var id in new[] { ExistingSolutionId, ExistingAdditionalServiceId, ExistingAssociatedServiceId })
            {
                context.OrderItems.Add(new OrderItem
                {
                    OrderId = OrderId,
                    Created = DateTime.Now,
                    CatalogueItem = context.CatalogueItems.First(x => x.Id == id),
                });
            }

            context.SaveChanges();
        }
    }
}
