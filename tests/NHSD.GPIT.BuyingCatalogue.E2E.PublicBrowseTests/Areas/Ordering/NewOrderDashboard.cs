﻿using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class NewOrderDashboard
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly Dictionary<string, string> Parameters = new() { { "OdsCode", "03F" } };

        public NewOrderDashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderController),
                  nameof(OrderController.NewOrder),
                  Parameters)
        {
        }

        [Fact]
        public void NewOrderDashboard_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("New Order");

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.TaskList)
                .Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.OrderDescriptionLink)
                .Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.OrderDescriptionStatus)
                .Should().BeTrue();
        }

        [Fact]
        public void NewOrderDashboard_ClickOrderDescription()
        {
            OrderingPages.OrderDashboard.ClickOrderDescriptionLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderDescriptionController),
                nameof(OrderDescriptionController.NewOrderDescription))
                    .Should().BeTrue();
        }
    }
}
