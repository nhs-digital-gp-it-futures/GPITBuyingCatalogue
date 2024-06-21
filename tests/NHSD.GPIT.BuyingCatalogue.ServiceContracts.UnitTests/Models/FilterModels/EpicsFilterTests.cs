using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models.FilterModels
{
    public static class EpicsFilterTests
    {
        [Theory]
        [MockAutoData]
        public static void EpicsFilter_DisplayText_ExpectedResult(EpicsFilter model)
        {
            model.DisplayText.Should().Be($"{model.Name} ({model.Count})");
        }
    }
}
