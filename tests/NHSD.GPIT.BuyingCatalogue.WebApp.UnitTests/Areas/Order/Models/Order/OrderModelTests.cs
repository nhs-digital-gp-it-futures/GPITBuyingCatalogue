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
        [CommonInlineAutoData(false, true, false, true, false)]
        [CommonInlineAutoData(true, true, false, true, true)]
        [CommonInlineAutoData(false, false, false, true, true)]
        [CommonInlineAutoData(false, true, true, true, true)]
        [CommonInlineAutoData(false, true, false, false, true)]
        [CommonInlineAutoData(true, false, true, false, true)]
        public static void CompletedOrder_SupportingDocumentsRequired(
            bool hasSpecificRequirements,
            bool useDefaultBilling,
            bool useDefaultDataProcessing,
            bool useDefaultImplementationPlan,
            bool expectedOutput,
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.ContractFlags.HasSpecificRequirements = hasSpecificRequirements;
            order.ContractFlags.UseDefaultBilling = useDefaultBilling;
            order.ContractFlags.UseDefaultDataProcessing = useDefaultDataProcessing;
            order.ContractFlags.UseDefaultImplementationPlan = useDefaultImplementationPlan;
            var model = new CompletedModel(internalOrgId, order);

            Assert.Equal(model.SupportingDocumentsRequired, expectedOutput);
        }
    }
}
