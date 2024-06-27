using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageOrders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ManageOrders
{
    public static class ManageOrdersDashboardModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Construct_MapsCorrectly(
            List<AdminManageOrder> orders,
            List<FrameworkFilterInfo> frameworks,
            PageOptions options,
            OrderStatus status,
            string framework)
        {
            var model = new ManageOrdersDashboardModel(orders, frameworks, options, status, framework);

            model.Orders.Should().BeEquivalentTo(orders);
            model.Options.Should().BeEquivalentTo(options);
            foreach (var item in frameworks)
            {
                model.AvailableFrameworks.Should().ContainEquivalentOf(new SelectOption<string>
                {
                    Value = item.Id,
                    Text = $"{item.ShortName}{(item.Expired ? " (expired)" : string.Empty)}",
                });
            }

            model.SelectedStatus.Should().Be(status);
            model.SelectedFramework.Should().Be(framework);
        }

        [Theory]
        [MockAutoData]
        public static void SelectedStatus_Empty_SetsCorrectly(
            List<AdminManageOrder> orders,
            List<FrameworkFilterInfo> frameworks,
            PageOptions options,
            string framework)
        {
            var model = new ManageOrdersDashboardModel(orders, frameworks, options, null, framework);
            model.SelectedStatus.Should().BeNull();
        }

        [Theory]
        [MockInlineAutoData(null, null, 0)]
        [MockInlineAutoData(null, OrderStatus.Completed, 1)]
        [MockInlineAutoData("test", null, 1)]
        [MockInlineAutoData("test", OrderStatus.Completed, 2)]
        public static void FilterCount_SetsCorrectly(
            string framework,
            OrderStatus? status,
            int count,
            List<AdminManageOrder> orders,
            List<FrameworkFilterInfo> frameworks,
            PageOptions options)
        {
            var model = new ManageOrdersDashboardModel(orders, frameworks, options, status, framework);
            model.FilterCount.Should().Be(count);
        }
    }
}
