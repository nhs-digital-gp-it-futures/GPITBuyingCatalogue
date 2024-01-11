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
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.LastUpdatedByUser = aspNetUser;

            var model = new OrderModel(internalOrgId, order, progress);

            model.Progress.Should().BeEquivalentTo(progress);
            model.Title.Should().Be($"Order {order.CallOffId}");
            model.CallOffId.Should().Be(order.CallOffId);
            model.Description.Should().Be(order.Description);
            model.LastUpdatedByUserName.Should().Be(aspNetUser.FullName);
            model.LastUpdated.Should().Be(order.LastUpdated);
            model.IsAmendment.Should().Be(order.IsAmendment);
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoOrder_PropertiesCorrectlySet(
            string internalOrgId,
            OrderProgress progress)
        {
            var model = new OrderModel(internalOrgId, null, progress);

            model.Progress.Should().BeEquivalentTo(progress);
            model.Title.Should().Be("New order");
            model.CallOffId.Should().BeEquivalentTo(default(CallOffId));
            model.Description.Should().Be(default);
        }

        [Theory]
        [CommonAutoData]
        public static void Construct_IsAmendment_SetsTitle(
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            const string expected =
                "You can amend parts of this order as required and will need to review other parts that cannot be changed. Your amendments will be saved as you progress through each section.";

            order.LastUpdatedByUser = aspNetUser;
            order.Revision = 2;

            var model = new OrderModel(internalOrgId, order, progress);

            model.TitleAdvice.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void Construct_OriginalOrder_SetsTitle(
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

            var model = new OrderModel(internalOrgId, order, progress);

            model.TitleAdvice.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void Construct_CompetitionOrder_SetsTitle(
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

            var model = new OrderModel(internalOrgId, order, progress);

            model.TitleAdvice.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(OrderSummaryField.OrderDescription, "Update the description of this order if needed.")]
        [CommonInlineAutoData(OrderSummaryField.OrderingParty, "Change the primary contact for this order if needed.")]
        [CommonInlineAutoData(OrderSummaryField.Supplier, "Select a different supplier contact if needed.")]
        [CommonInlineAutoData(OrderSummaryField.CommencementDate, "Review the commencement date, maximum term and initial period for this contract.")]
        [CommonInlineAutoData(OrderSummaryField.ServiceRecipients, "Select the organisations you want to receive this solution.")]
        [CommonInlineAutoData(OrderSummaryField.SolutionsAndServices, "Select a solution or services, prices and quantities.")]
        [CommonInlineAutoData(OrderSummaryField.PlannedDeliveryDates, "Enter the planned delivery dates for the items you're ordering.")]
        [CommonInlineAutoData(OrderSummaryField.FundingSources, "Allocate funding sources for items in this order.")]
        [CommonInlineAutoData(OrderSummaryField.ImplementationPlan, "Review the default milestones that will act as payment triggers and create bespoke ones.")]
        [CommonInlineAutoData(OrderSummaryField.DataProcessing, "Download the data processing information template for the supplier to complete.")]
        [CommonInlineAutoData(OrderSummaryField.ReviewAndComplete, "Check the information you’ve provided is correct and complete this amended order.")]
        public static void StatusDescription_IsAmendment_ReturnsExpected(
            OrderSummaryField summaryField,
            string expected,
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.LastUpdatedByUser = aspNetUser;
            order.CompetitionId = null;
            order.Revision = 2;

            var model = new OrderModel(internalOrgId, order, progress);

            model.StatusDescription(summaryField).Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(OrderSummaryField.OrderDescription, "Provide a description of your order.")]
        [CommonInlineAutoData(OrderSummaryField.OrderingParty, "Provide information about the primary contact for your order.")]
        [CommonInlineAutoData(OrderSummaryField.Supplier, "Find the supplier you want to order from and select a supplier contact.")]
        [CommonInlineAutoData(OrderSummaryField.CommencementDate, "Provide the commencement date, the length of the contract and its initial period.")]
        [CommonInlineAutoData(OrderSummaryField.ServiceRecipients, "Select the organisations you want to receive this solution.")]
        [CommonInlineAutoData(OrderSummaryField.SolutionsAndServices, "Select a solution or services, prices and quantities.")]
        [CommonInlineAutoData(OrderSummaryField.PlannedDeliveryDates, "Enter the planned delivery dates for the items you're ordering.")]
        [CommonInlineAutoData(OrderSummaryField.FundingSources, "Review how you’ll be paying for your order.")]
        [CommonInlineAutoData(OrderSummaryField.ImplementationPlan, "Review the default milestones that will act as payment triggers and create bespoke ones.")]
        [CommonInlineAutoData(OrderSummaryField.AssociatedServicesBilling, "Review the default milestones, create bespoke ones and add specific requirements.")]
        [CommonInlineAutoData(OrderSummaryField.DataProcessing, "Download the data processing information template for the supplier to complete.")]
        [CommonInlineAutoData(OrderSummaryField.ReviewAndComplete, "Check the information you’ve provided is correct and complete your order.")]
        public static void StatusDescription_IsOriginalOrder_ReturnsExpected(
            OrderSummaryField summaryField,
            string expected,
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.LastUpdatedByUser = aspNetUser;
            order.CompetitionId = null;
            order.Revision = 1;

            var model = new OrderModel(internalOrgId, order, progress);

            model.StatusDescription(summaryField).Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(OrderSummaryField.OrderDescription, "Edit the description for this order if needed.")]
        [CommonInlineAutoData(OrderSummaryField.OrderingParty, "Provide information about the primary contact for your order.")]
        [CommonInlineAutoData(OrderSummaryField.Supplier, "Provide information about the supplier contact for your order.")]
        [CommonInlineAutoData(OrderSummaryField.CommencementDate, "Review the maximum term of your contract and provide a commencement date and initial period.")]
        [CommonInlineAutoData(OrderSummaryField.ServiceRecipients, "Review the organisations you’re ordering for.")]
        [CommonInlineAutoData(OrderSummaryField.SolutionsAndServices, "Review the items you’re ordering and their prices and quantities.")]
        [CommonInlineAutoData(OrderSummaryField.PlannedDeliveryDates, "Enter the planned delivery dates for the items you're ordering.")]
        [CommonInlineAutoData(OrderSummaryField.FundingSources, "Review how you’ll be paying for your order.")]
        [CommonInlineAutoData(OrderSummaryField.ImplementationPlan, "Review the default milestones that will act as payment triggers and create bespoke ones.")]
        [CommonInlineAutoData(OrderSummaryField.AssociatedServicesBilling, "Review the default milestones, create bespoke ones and add specific requirements.")]
        [CommonInlineAutoData(OrderSummaryField.DataProcessing, "Download the data processing information template for the supplier to complete.")]
        [CommonInlineAutoData(OrderSummaryField.ReviewAndComplete, "Check the information you’ve provided is correct and complete your order.")]
        public static void StatusDescription_IsCompetitionOrder_ReturnsExpected(
            OrderSummaryField summaryField,
            string expected,
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.LastUpdatedByUser = aspNetUser;
            order.CompetitionId = 1;
            order.Revision = 1;

            var model = new OrderModel(internalOrgId, order, progress);

            model.StatusDescription(summaryField).Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void StatusDescription_InvalidSummaryField_ReturnsExpected(
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.LastUpdatedByUser = aspNetUser;
            order.CompetitionId = 1;
            order.Revision = 1;

            var model = new OrderModel(internalOrgId, order, progress);

            model.StatusDescription((OrderSummaryField)int.MaxValue).Should().BeEmpty();
        }
    }
}
