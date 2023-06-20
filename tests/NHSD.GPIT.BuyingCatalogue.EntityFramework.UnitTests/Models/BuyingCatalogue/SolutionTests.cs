using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    public static class SolutionTests
    {
        [Fact]
        public static void ApplicationType_Should_Be_Null()
        {
            var solutuion = new Solution();
            solutuion.ApplicationTypeDetail.Should().BeNull();
        }

        [Fact]
        public static void EnsureAndGetApplicationType_Should_Return_Instance()
        {
            var solutuion = new Solution();
            solutuion.EnsureAndGetApplicationType().Should().NotBeNull();
        }
    }
}
