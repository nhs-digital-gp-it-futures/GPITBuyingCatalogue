using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models.FilterModels
{
    public static class EpicsFilterTests
    {
        [Theory]
        [CommonAutoData]
        public static void EpicsFilter_DisplayText_ExpectedResult(EpicsFilter model)
        {
            model.DisplayText.Should().Be($"{model.Name} ({model.Count})");
        }
    }
}
