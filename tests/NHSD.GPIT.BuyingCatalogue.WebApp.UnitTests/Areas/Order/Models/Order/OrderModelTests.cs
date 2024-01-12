using System.Collections.Generic;
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
            model.IsAmendment.Should().Be(order.IsAmendment);
        }

        [Theory]
        [CommonAutoData]
        public static void InProgressOrder_IsAmendment_SetsTitle(
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            const string expected =
                "You can amend parts of this order as required and will need to review other parts that cannot be changed. Your amendments will be saved as you progress through each section.";

            order.LastUpdatedByUser = aspNetUser;
            order.Revision = 2;

            var model = new OrderModel(internalOrgId, progress, order);

            model.TitleAdvice.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void InProgressOrder_OriginalOrder_SetsTitle(
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            const string expected =
                "Complete the following steps to create an order summary.";

            order.LastUpdatedByUser = aspNetUser;
            order.CompetitionId = null;
            order.Revision = 1;

            var model = new OrderModel(internalOrgId, progress, order);

            model.TitleAdvice.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void InProgressOrder_CompetitionOrder_SetsTitle(
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            const string expected =
                "The information you included in your competition has already been added. Your progress will be saved as you complete each section.";

            order.LastUpdatedByUser = aspNetUser;
            order.CompetitionId = 1;
            order.Revision = 1;

            var model = new OrderModel(internalOrgId, progress, order);

            model.TitleAdvice.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(OrderSummaryField.OrderDescription, true)]
        [CommonInlineAutoData(OrderSummaryField.OrderingParty, true)]
        [CommonInlineAutoData(OrderSummaryField.Supplier, true)]
        [CommonInlineAutoData(OrderSummaryField.CommencementDate, true)]
        [CommonInlineAutoData(OrderSummaryField.ServiceRecipients, false)]
        [CommonInlineAutoData(OrderSummaryField.SolutionsAndServices, false)]
        [CommonInlineAutoData(OrderSummaryField.PlannedDeliveryDates, false)]
        [CommonInlineAutoData(OrderSummaryField.FundingSources, true)]
        [CommonInlineAutoData(OrderSummaryField.ImplementationPlan, false)]
        [CommonInlineAutoData(OrderSummaryField.AssociatedServicesBilling, false)]
        [CommonInlineAutoData(OrderSummaryField.DataProcessing, false)]
        [CommonInlineAutoData(OrderSummaryField.ReviewAndComplete, true)]
        public static void InProgressOrder_StausDecription_IsAmendment_ReturnsExpected(
            OrderSummaryField key,
            bool amendmentSpecific,
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.OrderType = OrderTypeEnum.Solution;
            order.LastUpdatedByUser = aspNetUser;
            order.CompetitionId = null;
            order.Revision = 2;

            var model = new OrderModel(internalOrgId, progress, order);

            if (amendmentSpecific)
            {
                model.StatusDescription(key)
                    .Should().Be(OrderModel.AmendmentSpecificDescriptions.Value(key));
            }
            else
            {
                model.StatusDescription(key)
                    .Should().Be(OrderModel.DefaultDescriptions.Value(key));
            }
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.OrderDescription)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.OrderingParty)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.Supplier)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.CommencementDate)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ServiceRecipients)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.SolutionsAndServices)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.PlannedDeliveryDates)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.FundingSources)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ImplementationPlan)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.AssociatedServicesBilling)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.DataProcessing)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ReviewAndComplete)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.OrderDescription)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.OrderingParty)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.Supplier)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.CommencementDate)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ServiceRecipients)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.SolutionsAndServices)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.PlannedDeliveryDates)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.FundingSources)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ImplementationPlan)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.AssociatedServicesBilling)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.DataProcessing)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ReviewAndComplete)]
        public static void InProgressOrder_StausDecription_IsOriginalOrder_ReturnsExpected(
            OrderTypeEnum orderType,
            OrderSummaryField key,
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.OrderType = orderType;
            order.LastUpdatedByUser = aspNetUser;
            order.CompetitionId = null;
            order.Revision = 1;

            var model = new OrderModel(internalOrgId, progress, order);

            model.StatusDescription(key)
                .Should().Be(OrderModel.DefaultDescriptions.Value(key));
        }

        [Theory]
        [CommonInlineAutoData(OrderSummaryField.OrderDescription, true)]
        [CommonInlineAutoData(OrderSummaryField.OrderingParty, false)]
        [CommonInlineAutoData(OrderSummaryField.Supplier, true)]
        [CommonInlineAutoData(OrderSummaryField.CommencementDate, true)]
        [CommonInlineAutoData(OrderSummaryField.ServiceRecipients, true)]
        [CommonInlineAutoData(OrderSummaryField.SolutionsAndServices, true)]
        [CommonInlineAutoData(OrderSummaryField.PlannedDeliveryDates, false)]
        [CommonInlineAutoData(OrderSummaryField.FundingSources, false)]
        [CommonInlineAutoData(OrderSummaryField.ImplementationPlan, false)]
        [CommonInlineAutoData(OrderSummaryField.AssociatedServicesBilling, false)]
        [CommonInlineAutoData(OrderSummaryField.DataProcessing, false)]
        [CommonInlineAutoData(OrderSummaryField.ReviewAndComplete, false)]
        public static void InProgressOrder_StausDecription_IsCompetitionOrder_ReturnsExpected(
            OrderSummaryField key,
            bool competitionSpecific,
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.OrderType = OrderTypeEnum.Solution;
            order.LastUpdatedByUser = aspNetUser;
            order.CompetitionId = 1;
            order.Revision = 1;

            var model = new OrderModel(internalOrgId, progress, order);

            if (competitionSpecific)
            {
                model.StatusDescription(key)
                    .Should().Be(OrderModel.CompetitionOrderDescriptions.Value(key));
            }
            else
            {
                model.StatusDescription(key)
                    .Should().Be(OrderModel.DefaultDescriptions.Value(key));
            }
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.OrderDescription, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.OrderingParty, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.Supplier, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.CommencementDate, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ServiceRecipients, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.SolutionsAndServices, true)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.PlannedDeliveryDates, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.FundingSources, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ImplementationPlan, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.AssociatedServicesBilling, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.DataProcessing, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ReviewAndComplete, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.OrderDescription, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.OrderingParty, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.Supplier, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.CommencementDate, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ServiceRecipients, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.SolutionsAndServices, true)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.PlannedDeliveryDates, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.FundingSources, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ImplementationPlan, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.AssociatedServicesBilling, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.DataProcessing, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ReviewAndComplete, false)]
        public static void InProgressOrder_StausDecription_MergerSplitOrder_ReturnsExpected(
            OrderTypeEnum orderType,
            OrderSummaryField key,
            bool mergerSplitSpecific,
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.OrderType = orderType;
            order.LastUpdatedByUser = aspNetUser;
            order.CompetitionId = null;
            order.Revision = 1;

            var model = new OrderModel(internalOrgId, progress, order);

            if (mergerSplitSpecific)
            {
                model.StatusDescription(key)
                    .Should().Be(OrderModel.MergerSplitSpecificDescriptions.Value(key));
            }
            else
            {
                model.StatusDescription(key)
                    .Should().Be(OrderModel.DefaultDescriptions.Value(key));
            }
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
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.OrderDescription)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.OrderingParty)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.Supplier)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.CommencementDate)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ServiceRecipients)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.SolutionsAndServices)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.PlannedDeliveryDates)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.FundingSources)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ImplementationPlan)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.AssociatedServicesBilling)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.DataProcessing)]
        [CommonInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ReviewAndComplete)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.OrderDescription)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.OrderingParty)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.Supplier)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.CommencementDate)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ServiceRecipients)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.SolutionsAndServices)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.PlannedDeliveryDates)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.FundingSources)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ImplementationPlan)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.AssociatedServicesBilling)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.DataProcessing)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ReviewAndComplete)]
        public static void NewOrder_StausDecription_IsOriginalOrder_ReturnsExpected(
            OrderTypeEnum orderType,
            OrderSummaryField key,
            string internalOrgId,
            OrderProgress progress,
            string organisationName)
        {
            var model = new OrderModel(internalOrgId, orderType, progress, organisationName);

            model.StatusDescription(key)
                .Should().Be(OrderModel.DefaultDescriptions.Value(key));
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.OrderDescription, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.OrderingParty, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.Supplier, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.CommencementDate, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ServiceRecipients, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.SolutionsAndServices, true)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.PlannedDeliveryDates, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.FundingSources, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ImplementationPlan, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.AssociatedServicesBilling, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.DataProcessing, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ReviewAndComplete, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.OrderDescription, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.OrderingParty, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.Supplier, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.CommencementDate, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ServiceRecipients, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.SolutionsAndServices, true)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.PlannedDeliveryDates, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.FundingSources, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ImplementationPlan, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.AssociatedServicesBilling, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.DataProcessing, false)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ReviewAndComplete, false)]
        public static void NewOrder_StausDecription_MergerSplitOrder_ReturnsExpected(
            OrderTypeEnum orderType,
            OrderSummaryField key,
            bool mergerSplitSpecific,
            string internalOrgId,
            OrderProgress progress,
            string organisationName)
        {
            var model = new OrderModel(internalOrgId, orderType, progress, organisationName);

            if (mergerSplitSpecific)
            {
                model.StatusDescription(key)
                    .Should().Be(OrderModel.MergerSplitSpecificDescriptions.Value(key));
            }
            else
            {
                model.StatusDescription(key)
                    .Should().Be(OrderModel.DefaultDescriptions.Value(key));
            }
        }

        public static string Value(this IEnumerable<KeyValuePair<OrderSummaryField, string>> source, OrderSummaryField key)
        {
            return source.Single(kv => kv.Key == key).Value;
        }
    }
}
