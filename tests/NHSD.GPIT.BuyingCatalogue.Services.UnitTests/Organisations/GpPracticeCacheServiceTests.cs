using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Organisations
{
    public static class GpPracticeCacheServiceTests
    {
        private const string OdsCode = "A00001";
        private const int NumberOfPatients = 1234;

        private static readonly DateTime ExtractDate = new(2000, 1, 1);

        private static readonly GpPracticeSize GpPracticeSize = new()
        {
            ExtractDate = ExtractDate,
            OdsCode = OdsCode,
            NumberOfPatients = NumberOfPatients,
        };

        [Theory]
        [CommonAutoData]
        public static async Task GetNumberOfPatients_CacheReturnsValue_ReturnsValue(
            [Frozen] Mock<IGpPracticeCache> mockCache,
            GpPracticeCacheService systemUnderTest)
        {
            mockCache
                .Setup(x => x.Get(OdsCode))
                .Returns(NumberOfPatients);

            var result = await systemUnderTest.GetNumberOfPatients(OdsCode);

            result.Should().Be(NumberOfPatients);
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetNumberOfPatients_CacheReturnsNotFound_ReturnsNull(
            [Frozen] Mock<IGpPracticeCache> mockCache,
            GpPracticeCacheService systemUnderTest)
        {
            mockCache
                .Setup(x => x.Get(OdsCode))
                .Returns(GpPracticeCacheService.EntryNotFound);

            var result = await systemUnderTest.GetNumberOfPatients(OdsCode);

            result.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetNumberOfPatients_NotInCache_FoundInDb_ReturnsValue(
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] Mock<IGpPracticeCache> mockCache,
            GpPracticeCacheService systemUnderTest)
        {
            mockCache
                .Setup(x => x.Get(OdsCode))
                .Returns((int?)null);

            dbContext.GpPracticeSizes.Add(GpPracticeSize);
            await dbContext.SaveChangesAsync();

            var result = await systemUnderTest.GetNumberOfPatients(OdsCode);

            mockCache.Verify(x => x.Set(OdsCode, NumberOfPatients));

            result.Should().Be(NumberOfPatients);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetNumberOfPatients_NotInCache_NotInDb_ReturnsNull(
            [Frozen] Mock<IGpPracticeCache> mockCache,
            GpPracticeCacheService systemUnderTest)
        {
            mockCache
                .Setup(x => x.Get(OdsCode))
                .Returns((int?)null);

            var result = await systemUnderTest.GetNumberOfPatients(OdsCode);

            mockCache.Verify(x => x.Set(OdsCode, -1));

            result.Should().BeNull();
        }
    }
}
