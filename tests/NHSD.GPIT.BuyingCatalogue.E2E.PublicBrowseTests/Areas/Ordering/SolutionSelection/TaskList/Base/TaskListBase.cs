using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList.Base
{
    [Collection(nameof(OrderingCollection))]
    public abstract class TaskListBase : BuyerTestBase
    {
        protected TaskListBase(LocalWebApplicationFactory factory, Dictionary<string, string> parameters)
            : base(factory, typeof(TaskListController), nameof(TaskListController.TaskList), parameters)
        {
        }

        protected abstract string PageTitle { get; }

        protected virtual bool AdditionalServicesDisplayed => true;

        protected virtual bool AssociatedServicesDisplayed => true;

        protected virtual bool ChangeCatalogueSolutionLinkVisible => true;

        protected virtual bool ChangeAdditionalServicesLinkVisible => true;

        protected virtual bool ChangeAssociatedServicesLinkVisible => true;

        protected abstract List<TaskListOrderItem> OrderItems { get; }

        protected virtual Type ContinueController => typeof(OrderController);

        protected virtual string ContinueAction => nameof(OrderController.Order);

        protected virtual string ChangeSolutionAction => nameof(CatalogueSolutionsController.EditSolution);

        [Fact]
        public void TaskList_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo(PageTitle.FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(TaskListObjects.SolutionDetails).Should().Be(true);
            CommonActions.ElementIsDisplayed(TaskListObjects.ChangeSolutionLink).Should().Be(ChangeCatalogueSolutionLinkVisible);
            CommonActions.ElementIsDisplayed(TaskListObjects.AdditionalServiceDetails).Should().Be(AdditionalServicesDisplayed);
            CommonActions.ElementIsDisplayed(TaskListObjects.ChangeAdditionalServicesLink).Should().Be(ChangeAdditionalServicesLinkVisible);
            CommonActions.ElementIsDisplayed(TaskListObjects.AssociatedServiceDetails).Should().Be(AssociatedServicesDisplayed);
            CommonActions.ElementIsDisplayed(TaskListObjects.ChangeAssociatedServicesLink).Should().Be(ChangeAssociatedServicesLinkVisible);
            CommonActions.ElementIsDisplayed(TaskListObjects.ContinueButton).Should().BeTrue();

            foreach (var orderItem in OrderItems)
            {
                CommonActions.ElementIsDisplayed(TaskListObjects.Name(orderItem.CatalogueItemId)).Should().BeTrue();
                CommonActions.ElementIsDisplayed(TaskListObjects.ServiceRecipientsLink(orderItem.CatalogueItemId)).Should().BeTrue();
                CommonActions.ElementIsDisplayed(TaskListObjects.PriceLink(orderItem.CatalogueItemId)).Should().Be(orderItem.PriceLinkActive);
                CommonActions.ElementIsDisplayed(TaskListObjects.QuantityLink(orderItem.CatalogueItemId)).Should().Be(orderItem.QuantityLinkActive);
                CommonActions.ElementTextEqualTo(TaskListObjects.Name(orderItem.CatalogueItemId), orderItem.Name).Should().BeTrue();
            }
        }

        [Fact]
        public void TaskList_ClickContinueLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(TaskListObjects.ContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(ContinueController, ContinueAction).Should().BeTrue();
        }

        [Fact]
        public void TaskList_ClickChangeCatalogueSolutionLink_ExpectedResult()
        {
            if (!ChangeCatalogueSolutionLinkVisible)
            {
                return;
            }

            CommonActions.ClickLinkElement(TaskListObjects.ChangeSolutionLink);

            CommonActions.PageLoadedCorrectGetIndex(typeof(CatalogueSolutionsController), ChangeSolutionAction).Should().BeTrue();
        }

        [Fact]
        public void TaskList_ClickChangeAdditionalServicesLink_ExpectedResult()
        {
            if (!ChangeAdditionalServicesLinkVisible)
            {
                return;
            }

            CommonActions.ClickLinkElement(TaskListObjects.ChangeAdditionalServicesLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectAdditionalServices)).Should().BeTrue();
        }

        [Fact]
        public void TaskList_ClickChangeAssociatedServicesLink_ExpectedResult()
        {
            if (!ChangeAssociatedServicesLinkVisible)
            {
                return;
            }

            CommonActions.ClickLinkElement(TaskListObjects.ChangeAssociatedServicesLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.SelectAssociatedServices)).Should().BeTrue();
        }

        [Fact]
        public void TaskList_ServiceRecipientsLink_ExpectedResult()
        {
            foreach (var orderItem in OrderItems)
            {
                CommonActions.ClickLinkElement(TaskListObjects.ServiceRecipientsLink(orderItem.CatalogueItemId));

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ServiceRecipientsController),
                    orderItem.ServiceRecipientsAction).Should().BeTrue();

                CommonActions.ClickGoBackLink();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(TaskListController),
                    nameof(TaskListController.TaskList)).Should().BeTrue();
            }
        }

        [Fact]
        public void TaskList_PriceLink_ExpectedResult()
        {
            foreach (var orderItem in OrderItems.Where(x => x.PriceLinkActive))
            {
                CommonActions.ClickLinkElement(TaskListObjects.PriceLink(orderItem.CatalogueItemId));

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(PricesController),
                    orderItem.PriceAction).Should().BeTrue();

                CommonActions.ClickGoBackLink();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(TaskListController),
                    nameof(TaskListController.TaskList)).Should().BeTrue();
            }
        }

        [Fact]
        public void TaskList_QuantityLink_ExpectedResult()
        {
            foreach (var orderItem in OrderItems.Where(x => x.QuantityLinkActive))
            {
                CommonActions.ClickLinkElement(TaskListObjects.QuantityLink(orderItem.CatalogueItemId));

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(QuantityController),
                    orderItem.QuantityAction).Should().BeTrue();

                CommonActions.ClickGoBackLink();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(TaskListController),
                    nameof(TaskListController.TaskList)).Should().BeTrue();
            }
        }
    }
}
