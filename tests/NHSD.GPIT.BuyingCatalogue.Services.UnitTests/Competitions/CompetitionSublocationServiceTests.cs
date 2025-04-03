using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
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

        public static IEnumerable<object[]> CompetitionSublocationsWithRecipientsForCount()
        {
            return [[new List<CompetitionSublocation>()]];
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetCountForCompetitionSublocationRecipients_ReturnsCount(
            List<CompetitionSublocation> competitionSublocationsWithRecipients,
            int expectedQuantity,
            Competition competition,
            Organisation organisation,
            [Frozen] BuyingCatalogueDbContext context,
            CompetitionSublocationService service)
        {
            Assert.Fail("Not implemented");
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
