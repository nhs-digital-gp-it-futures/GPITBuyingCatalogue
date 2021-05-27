using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using NUnit.Framework;

// MJRTODO - Getting a namespace vs type clash when using Solution
namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.SolutionX
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionStatusModelTests
    {
        [Test]
        public static void Calls_Inherits_MarketingDisplayBaseModel()
        {
            typeof(SolutionStatusModel)
                .Should()
                .BeAssignableTo<MarketingDisplayBaseModel>();
        }
    }
}
