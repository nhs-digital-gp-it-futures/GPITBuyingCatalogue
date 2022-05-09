using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class PublicBrowseScenarios : AnonymousTestBase, IClassFixture<WebApplicationConnector>
    {
        public PublicBrowseScenarios(WebApplicationConnector connector, ITestOutputHelper testOutputHelper)
           : base(connector, typeof(HomeController), nameof(HomeController.ContactUs), null, testOutputHelper)
        {
        }

        [Fact]
        public void PBTest_DoesThing()
        {

            int a = 1;
            int b = 2;

            // uri.ToString().Should().Be("abc");

        }
    }
}
