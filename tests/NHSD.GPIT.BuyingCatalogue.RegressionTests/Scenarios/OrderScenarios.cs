using System;
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
           : base(connector, typeof(OrderController), nameof(OrderController.NewOrder), Parameters, testOutputHelper)
        {
        }

        [Fact]
        public void FirstTest_DoesThing()
        {

            int a = 1;
            int b = 2;

            BuyerUsername.Should().Be("abc");
        }
    }
}
