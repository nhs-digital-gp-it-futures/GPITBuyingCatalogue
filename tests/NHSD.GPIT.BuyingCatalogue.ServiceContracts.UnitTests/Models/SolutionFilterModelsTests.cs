using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models
{
    public static class SolutionFilterModelsTests
    {
        [Theory]
        [CommonAutoData]
        public static void CapabilitiesFilter_Input_ExpectedResult(string name, int count)
        {
            var capabilitiesFilter = new CapabilitiesFilter()
            {
                Name = name,
                Count = count,
            };

            capabilitiesFilter.DisplayText.Should().Be($"{name} ({count})");
        }

        [Theory]
        [CommonAutoData]
        public static void EpicsFilter_Input_ExpectedResult(string name, int count)
        {
            var epicsFilter = new EpicsFilter
            {
                Name = name,
                Count = count,
            };

            epicsFilter.DisplayText.Should().Be($"{name} ({count})");
        }
    }
}
