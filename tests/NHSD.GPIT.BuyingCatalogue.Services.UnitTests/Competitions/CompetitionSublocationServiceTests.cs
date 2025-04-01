using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Competitions
{
    public static class CompetitionSublocationServiceTests
    {
        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetCompetitionSublocationWithRecipients_ReturnsCompetitionSublocation(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddSublocationRecipients_RejectsNullArguments(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddSublocationRecipients_RejectsInvalidOperations(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddSublocationRecipients_AddsAsExpected(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task RemoveSublocationRecipients_RejectsNullArguments(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task RemoveSublocationRecipients_RejectsInvalidOperations(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task RemoveSublocationRecipients_RemovesAsExpected(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }
    }
}
