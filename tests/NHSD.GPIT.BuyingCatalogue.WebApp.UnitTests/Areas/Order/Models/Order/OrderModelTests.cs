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
        [MockAutoData]
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
        [MockAutoData]
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
        [MockAutoData]
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
        [MockAutoData]
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
        [MockInlineAutoData(OrderSummaryField.OrderDescription, true)]
        [MockInlineAutoData(OrderSummaryField.OrderingParty, true)]
        [MockInlineAutoData(OrderSummaryField.Supplier, true)]
        [MockInlineAutoData(OrderSummaryField.CommencementDate, true)]
        [MockInlineAutoData(OrderSummaryField.ServiceRecipients, false)]
        [MockInlineAutoData(OrderSummaryField.SolutionsAndServices, false)]
        [MockInlineAutoData(OrderSummaryField.PlannedDeliveryDates, false)]
        [MockInlineAutoData(OrderSummaryField.FundingSources, true)]
        [MockInlineAutoData(OrderSummaryField.ImplementationPlan, false)]
        [MockInlineAutoData(OrderSummaryField.AssociatedServicesBilling, false)]
        [MockInlineAutoData(OrderSummaryField.DataProcessing, false)]
        [MockInlineAutoData(OrderSummaryField.ReviewAndComplete, true)]
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
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.OrderDescription)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.OrderingParty)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.Supplier)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.CommencementDate)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ServiceRecipients)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.SolutionsAndServices)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.PlannedDeliveryDates)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.FundingSources)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ImplementationPlan)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.AssociatedServicesBilling)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.DataProcessing)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ReviewAndComplete)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.OrderDescription)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.OrderingParty)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.Supplier)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.CommencementDate)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ServiceRecipients)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.SolutionsAndServices)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.PlannedDeliveryDates)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.FundingSources)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ImplementationPlan)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.AssociatedServicesBilling)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.DataProcessing)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ReviewAndComplete)]
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
        [MockInlineAutoData(OrderSummaryField.OrderDescription, true)]
        [MockInlineAutoData(OrderSummaryField.OrderingParty, false)]
        [MockInlineAutoData(OrderSummaryField.Supplier, true)]
        [MockInlineAutoData(OrderSummaryField.CommencementDate, true)]
        [MockInlineAutoData(OrderSummaryField.ServiceRecipients, true)]
        [MockInlineAutoData(OrderSummaryField.SolutionsAndServices, true)]
        [MockInlineAutoData(OrderSummaryField.PlannedDeliveryDates, false)]
        [MockInlineAutoData(OrderSummaryField.FundingSources, false)]
        [MockInlineAutoData(OrderSummaryField.ImplementationPlan, false)]
        [MockInlineAutoData(OrderSummaryField.AssociatedServicesBilling, false)]
        [MockInlineAutoData(OrderSummaryField.DataProcessing, false)]
        [MockInlineAutoData(OrderSummaryField.ReviewAndComplete, false)]
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
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.OrderDescription, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.OrderingParty, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.Supplier, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.CommencementDate, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ServiceRecipients, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.SolutionsAndServices, true)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.PlannedDeliveryDates, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.FundingSources, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ImplementationPlan, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.AssociatedServicesBilling, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.DataProcessing, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ReviewAndComplete, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.OrderDescription, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.OrderingParty, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.Supplier, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.CommencementDate, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ServiceRecipients, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.SolutionsAndServices, true)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.PlannedDeliveryDates, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.FundingSources, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ImplementationPlan, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.AssociatedServicesBilling, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.DataProcessing, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ReviewAndComplete, false)]
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
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [MockInlineAutoData(OrderTypeEnum.Solution)]
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
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.OrderDescription)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.OrderingParty)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.Supplier)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.CommencementDate)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ServiceRecipients)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.SolutionsAndServices)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.PlannedDeliveryDates)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.FundingSources)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ImplementationPlan)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.AssociatedServicesBilling)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.DataProcessing)]
        [MockInlineAutoData(OrderTypeEnum.Solution, OrderSummaryField.ReviewAndComplete)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.OrderDescription)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.OrderingParty)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.Supplier)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.CommencementDate)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ServiceRecipients)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.SolutionsAndServices)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.PlannedDeliveryDates)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.FundingSources)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ImplementationPlan)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.AssociatedServicesBilling)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.DataProcessing)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther, OrderSummaryField.ReviewAndComplete)]
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
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.OrderDescription, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.OrderingParty, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.Supplier, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.CommencementDate, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ServiceRecipients, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.SolutionsAndServices, true)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.PlannedDeliveryDates, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.FundingSources, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ImplementationPlan, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.AssociatedServicesBilling, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.DataProcessing, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, OrderSummaryField.ReviewAndComplete, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.OrderDescription, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.OrderingParty, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.Supplier, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.CommencementDate, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ServiceRecipients, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.SolutionsAndServices, true)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.PlannedDeliveryDates, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.FundingSources, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ImplementationPlan, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.AssociatedServicesBilling, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.DataProcessing, false)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, OrderSummaryField.ReviewAndComplete, false)]
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
