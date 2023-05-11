using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ManageOrders
{
    public static class DeleteOrderModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            CallOffId callOffId)
        {
            var model = new DeleteOrderModel(callOffId);

            model.CallOffId.Should().Be(callOffId);
            model.IsAmendment.Should().Be(callOffId.IsAmendment);
        }

        [Theory]
        [CommonAutoData]
        public static void WithNullOrderDeletionApproval_PropertiesCorrectlySet(
            CallOffId callOffId)
        {
            var model = new DeleteOrderModel(callOffId);

            model.ApprovalDay.Should().BeNull();
            model.ApprovalMonth.Should().BeNull();
            model.ApprovalYear.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void ApprovalDate_InvalidDayMonthYear_ReturnsNull(
            CallOffId callOffId)
        {
            var model = new DeleteOrderModel(callOffId);
            model.ApprovalDay = "A";
            model.ApprovalMonth = "B";
            model.ApprovalYear = "C";

            model.ApprovalDate.Should().BeNull();
        }
    }
}
