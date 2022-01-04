using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class CapabilitiesViewModelTests
    {
        [Fact]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(CapabilitiesViewModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Fact]
        public static void Class_Inherits_INoNavModel()
        {
            typeof(CapabilitiesViewModel)
                .Should()
                .BeAssignableTo<INoNavModel>();
        }
    }
}
