﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class OrderScenarios : BuyerTestBase, IClassFixture<WebApplicationConnector>
    {
        private const string InternalOrgId = "CG-03F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public OrderScenarios(WebApplicationConnector connector, ITestOutputHelper testOutputHelper)
           : base(connector, typeof(DashboardController), nameof(DashboardController.Organisation), Parameters, testOutputHelper)
        {
        }

        [Fact]
        public void FirstTest_DoesThing()
        {

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectTriageOrderValue();

            OrderingPages.OrderingTriage.SelectIdentifiedOrder();
        }
    }
}
