using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
        [InMemoryDbAutoData]
        public static void GetNumberOfPatients_ValueExists_ReturnsValue(
            [Frozen] BuyingCatalogueDbContext dbContext,
            GpPracticeCacheService systemUnderTest)
        {
            dbContext.GpPracticeSizes.Add(GpPracticeSize);
            dbContext.SaveChanges();

            systemUnderTest.Refresh();

            var result = systemUnderTest.GetNumberOfPatients(OdsCode);

            result.Should().Be(NumberOfPatients);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GetNumberOfPatients_ValueDoesNotExist_ReturnsNull(
            GpPracticeCacheService systemUnderTest)
        {
            var result = systemUnderTest.GetNumberOfPatients(OdsCode);

            result.Should().BeNull();
        }
    }
}
