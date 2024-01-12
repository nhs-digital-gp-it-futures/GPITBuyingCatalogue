using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class OrderModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void InProgressOrder_WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.LastUpdatedByUser = aspNetUser;

            var model = new OrderModel(internalOrgId, progress, order);

            model.Progress.Should().BeEquivalentTo(progress);
            model.Title.Should().Be($"Order {order.CallOffId}");
            model.OrganisationName.Should().Be(order.OrderingParty.Name);
            model.CallOffId.Should().Be(order.CallOffId);
            model.Description.Should().Be(order.Description);
            model.LastUpdatedByUserName.Should().Be(aspNetUser.FullName);
            model.LastUpdated.Should().Be(order.LastUpdated);
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [CommonInlineAutoData(OrderTypeEnum.Solution)]
        public static void InProgressOrder_StausDecription_All_Default(
            OrderTypeEnum orderType,
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.OrderType = orderType;
            order.Revision = 1;
            order.LastUpdatedByUser = aspNetUser;

            var model = new OrderModel(internalOrgId, progress, order);

            model.StatusDescription(OrderSummaryField.OrderDescription)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.OrderDescription).Value);
            model.StatusDescription(OrderSummaryField.OrderingParty)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.OrderingParty).Value);
            model.StatusDescription(OrderSummaryField.Supplier)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.Supplier).Value);
            model.StatusDescription(OrderSummaryField.CommencementDate)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.CommencementDate).Value);
            model.StatusDescription(OrderSummaryField.ServiceRecipients)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.ServiceRecipients).Value);
            model.StatusDescription(OrderSummaryField.SolutionsAndServices)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.SolutionsAndServices).Value);
            model.StatusDescription(OrderSummaryField.PlannedDeliveryDates)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.PlannedDeliveryDates).Value);
            model.StatusDescription(OrderSummaryField.FundingSources)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.FundingSources).Value);
            model.StatusDescription(OrderSummaryField.ImplementationPlan)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.ImplementationPlan).Value);
            model.StatusDescription(OrderSummaryField.AssociatedServicesBilling)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.AssociatedServicesBilling).Value);
            model.StatusDescription(OrderSummaryField.DataProcessing)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.DataProcessing).Value);
            model.StatusDescription(OrderSummaryField.ReviewAndComplete)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.ReviewAndComplete).Value);
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.Solution)]
        public static void InProgressOrder_StausDecription_Amendment(
            OrderTypeEnum orderType,
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.OrderType = orderType;
            order.Revision = 2;
            order.LastUpdatedByUser = aspNetUser;

            var model = new OrderModel(internalOrgId, progress, order);

            model.StatusDescription(OrderSummaryField.OrderDescription)
                .Should()
                .Be(OrderModel.AmendmentSpecificDescriptions.Single(kv => kv.Key == OrderSummaryField.OrderDescription).Value);
            model.StatusDescription(OrderSummaryField.OrderingParty)
                .Should()
                .Be(OrderModel.AmendmentSpecificDescriptions.Single(kv => kv.Key == OrderSummaryField.OrderingParty).Value);
            model.StatusDescription(OrderSummaryField.Supplier)
                .Should()
                .Be(OrderModel.AmendmentSpecificDescriptions.Single(kv => kv.Key == OrderSummaryField.Supplier).Value);
            model.StatusDescription(OrderSummaryField.CommencementDate)
                .Should()
                .Be(OrderModel.AmendmentSpecificDescriptions.Single(kv => kv.Key == OrderSummaryField.CommencementDate).Value);
            model.StatusDescription(OrderSummaryField.ServiceRecipients)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.ServiceRecipients).Value);
            model.StatusDescription(OrderSummaryField.SolutionsAndServices)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.SolutionsAndServices).Value);
            model.StatusDescription(OrderSummaryField.PlannedDeliveryDates)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.PlannedDeliveryDates).Value);
            model.StatusDescription(OrderSummaryField.FundingSources)
                .Should()
                .Be(OrderModel.AmendmentSpecificDescriptions.Single(kv => kv.Key == OrderSummaryField.FundingSources).Value);
            model.StatusDescription(OrderSummaryField.ImplementationPlan)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.ImplementationPlan).Value);
            model.StatusDescription(OrderSummaryField.AssociatedServicesBilling)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.AssociatedServicesBilling).Value);
            model.StatusDescription(OrderSummaryField.DataProcessing)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.DataProcessing).Value);
            model.StatusDescription(OrderSummaryField.ReviewAndComplete)
                .Should()
                .Be(OrderModel.AmendmentSpecificDescriptions.Single(kv => kv.Key == OrderSummaryField.ReviewAndComplete).Value);
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [CommonInlineAutoData(OrderTypeEnum.Solution)]
        public static void NewOrder_WithValidArguments_PropertiesCorrectlySet(
            OrderTypeEnum orderType,
            string internalOrgId,
            OrderProgress progress,
            string organisationName)
        {
            var model = new OrderModel(internalOrgId, orderType, progress, organisationName);

            model.Progress.Should().BeEquivalentTo(progress);
            model.Title.Should().Be("New order");
            model.OrganisationName.Should().Be(organisationName);
            model.CallOffId.Should().BeEquivalentTo(default(CallOffId));
            model.Description.Should().Be(default);
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [CommonInlineAutoData(OrderTypeEnum.Solution)]
        public static void NewOrder_StatusDescription_All_Default(
            OrderTypeEnum orderType,
            string internalOrgId,
            OrderProgress progress,
            string organisationName)
        {
            var model = new OrderModel(internalOrgId, orderType, progress, organisationName);

            model.StatusDescription(OrderSummaryField.OrderDescription)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.OrderDescription).Value);
            model.StatusDescription(OrderSummaryField.OrderingParty)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.OrderingParty).Value);
            model.StatusDescription(OrderSummaryField.Supplier)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.Supplier).Value);
            model.StatusDescription(OrderSummaryField.CommencementDate)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.CommencementDate).Value);
            model.StatusDescription(OrderSummaryField.ServiceRecipients)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.ServiceRecipients).Value);
            model.StatusDescription(OrderSummaryField.SolutionsAndServices)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.SolutionsAndServices).Value);
            model.StatusDescription(OrderSummaryField.PlannedDeliveryDates)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.PlannedDeliveryDates).Value);
            model.StatusDescription(OrderSummaryField.FundingSources)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.FundingSources).Value);
            model.StatusDescription(OrderSummaryField.ImplementationPlan)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.ImplementationPlan).Value);
            model.StatusDescription(OrderSummaryField.AssociatedServicesBilling)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.AssociatedServicesBilling).Value);
            model.StatusDescription(OrderSummaryField.DataProcessing)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.DataProcessing).Value);
            model.StatusDescription(OrderSummaryField.ReviewAndComplete)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.ReviewAndComplete).Value);
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static void NewOrder_StatusDescription_Use_Some_Merger_And_Split_Descriptions(
            OrderTypeEnum orderType,
            string internalOrgId,
            OrderProgress progress,
            string organisationName)
        {
            var model = new OrderModel(internalOrgId, orderType, progress, organisationName);

            model.StatusDescription(OrderSummaryField.OrderDescription)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.OrderDescription).Value);
            model.StatusDescription(OrderSummaryField.OrderingParty)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.OrderingParty).Value);
            model.StatusDescription(OrderSummaryField.Supplier)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.Supplier).Value);
            model.StatusDescription(OrderSummaryField.CommencementDate)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.CommencementDate).Value);
            model.StatusDescription(OrderSummaryField.ServiceRecipients)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.ServiceRecipients).Value);
            model.StatusDescription(OrderSummaryField.SolutionsAndServices)
                .Should()
                .Be(OrderModel.MergerSplitSpecificDescriptions.Single(kv => kv.Key == OrderSummaryField.SolutionsAndServices).Value);
            model.StatusDescription(OrderSummaryField.PlannedDeliveryDates)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.PlannedDeliveryDates).Value);
            model.StatusDescription(OrderSummaryField.FundingSources)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.FundingSources).Value);
            model.StatusDescription(OrderSummaryField.ImplementationPlan)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.ImplementationPlan).Value);
            model.StatusDescription(OrderSummaryField.AssociatedServicesBilling)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.AssociatedServicesBilling).Value);
            model.StatusDescription(OrderSummaryField.DataProcessing)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.DataProcessing).Value);
            model.StatusDescription(OrderSummaryField.ReviewAndComplete)
                .Should()
                .Be(OrderModel.DefaultDescriptions.Single(kv => kv.Key == OrderSummaryField.ReviewAndComplete).Value);
        }
    }
}
