using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
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
    public static class GpPracticeServiceTests
    {
        private const string EmailAddress = "a@b.com";
        private const string OdsCode = "A00001";
        private const int NumberOfPatients = 1234;

        private static readonly DateTime ExtractDate = new(2000, 1, 1);

        private static readonly GpPracticeSize GpPracticeSize = new()
        {
            ExtractDate = ExtractDate,
            OdsCode = OdsCode,
            NumberOfPatients = NumberOfPatients,
        };

        private static readonly Uri Uri = new("http://www.test.com");

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(GpPracticeService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void ImportGpPracticeData_UriIsNull_ThrowsError(
            GpPracticeService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.ImportGpPracticeData(null, EmailAddress))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void ImportGpPracticeData_EmailAddressIsNull_ThrowsError(
            GpPracticeService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.ImportGpPracticeData(Uri, null))
                .Should().ThrowAsync<ArgumentNullException>();

            FluentActions
                .Awaiting(() => systemUnderTest.ImportGpPracticeData(Uri, string.Empty))
                .Should().ThrowAsync<ArgumentNullException>();

            FluentActions
                .Awaiting(() => systemUnderTest.ImportGpPracticeData(Uri, " "))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetNumberOfPatients_CacheReturnsPositiveValue_ReturnsValue(
            [Frozen] Mock<IGpPracticeCache> mockCache,
            GpPracticeService systemUnderTest)
        {
            mockCache
                .Setup(x => x.Get(OdsCode))
                .Returns(NumberOfPatients);

            var result = await systemUnderTest.GetNumberOfPatients(OdsCode);

            result.Should().Be(NumberOfPatients);
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetNumberOfPatients_CacheReturnsMinusOne_ReturnsNull(
            [Frozen] Mock<IGpPracticeCache> mockCache,
            GpPracticeService systemUnderTest)
        {
            mockCache
                .Setup(x => x.Get(OdsCode))
                .Returns(-1);

            var result = await systemUnderTest.GetNumberOfPatients(OdsCode);

            result.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetNumberOfPatients_NotInCache_FoundInDb_ReturnsValue(
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] Mock<IGpPracticeCache> mockCache,
            GpPracticeService systemUnderTest)
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
            GpPracticeService systemUnderTest)
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
