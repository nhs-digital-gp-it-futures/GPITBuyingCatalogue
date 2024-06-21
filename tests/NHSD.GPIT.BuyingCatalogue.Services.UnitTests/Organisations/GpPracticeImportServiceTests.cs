using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Organisations
{
    public static class GpPracticeImportServiceTests
    {
        private const string OdsCode = "A00001";
        private const int NumberOfPatients = 1234;

        private static readonly DateTime ExtractDate = new(2000, 1, 1);

        private static readonly GpPractice CsvData = new()
        {
            EXTRACT_DATE = ExtractDate,
            CODE = OdsCode,
            NUMBER_OF_PATIENTS = NumberOfPatients,
        };

        private static readonly Uri Uri = new("http://www.test.com");

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(GpPracticeImportService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task ImportGpPracticeData_ProviderReturnsNull_ReturnsError(
            [Frozen] IGpPracticeProvider mockProvider,
            GpPracticeImportService systemUnderTest)
        {
            mockProvider.GetGpPractices(Uri).Returns((IEnumerable<GpPractice>)null);

            var result = await systemUnderTest.PerformImport(Uri);

            result.Outcome.Should().Be(ImportGpPracticeListOutcome.CannotReadInputFile);
            result.ExtractDate.Should().BeNull();
            result.TotalRecords.Should().Be(0);
            result.TotalRecordsUpdated.Should().Be(0);
        }

        [Theory]
        [MockAutoData]
        public static async Task ImportGpPracticeData_ProviderThrowsError_ReturnsError(
            [Frozen] IGpPracticeProvider mockProvider,
            GpPracticeImportService systemUnderTest)
        {
            await mockProvider.GetGpPractices(Arg.Do<Uri>(x => throw new FormatException()));

            var result = await systemUnderTest.PerformImport(Uri);

            result.Outcome.Should().Be(ImportGpPracticeListOutcome.WrongFormat);
            result.ExtractDate.Should().BeNull();
            result.TotalRecords.Should().Be(0);
            result.TotalRecordsUpdated.Should().Be(0);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ImportGpPracticeData_EmptyDb_ImportsRecordFromProvider(
            [Frozen] IGpPracticeProvider mockProvider,
            GpPracticeImportService systemUnderTest)
        {
            mockProvider.GetGpPractices(Uri).Returns(new[] { CsvData });

            var result = await systemUnderTest.PerformImport(Uri);

            result.Outcome.Should().Be(ImportGpPracticeListOutcome.Success);
            result.ExtractDate.Should().Be(ExtractDate);
            result.TotalRecords.Should().Be(1);
            result.TotalRecordsUpdated.Should().Be(1);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ImportGpPracticeData_ProviderReturnsNewerRecordThanInDb_ImportsRecordFromProvider(
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] IGpPracticeProvider mockProvider,
            GpPracticeImportService systemUnderTest)
        {
            mockProvider.GetGpPractices(Uri).Returns(new[] { CsvData });

            dbContext.GpPracticeSizes.Add(new GpPracticeSize
            {
                ExtractDate = new DateTime(1990, 1, 1),
                OdsCode = OdsCode,
                NumberOfPatients = 1,
            });

            dbContext.SaveChanges();

            var result = await systemUnderTest.PerformImport(Uri);

            result.Outcome.Should().Be(ImportGpPracticeListOutcome.Success);
            result.ExtractDate.Should().Be(ExtractDate);
            result.TotalRecords.Should().Be(1);
            result.TotalRecordsUpdated.Should().Be(1);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ImportGpPracticeData_ProviderReturnsRecordWithSameExtractDateAsRecordInDb_DoesNotImportRecordFromProvider(
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] IGpPracticeProvider mockProvider,
            GpPracticeImportService systemUnderTest)
        {
            mockProvider.GetGpPractices(Uri).Returns(new[] { CsvData });

            dbContext.GpPracticeSizes.Add(new GpPracticeSize
            {
                ExtractDate = new DateTime(2000, 1, 1),
                OdsCode = OdsCode,
                NumberOfPatients = 1,
            });

            dbContext.SaveChanges();

            var result = await systemUnderTest.PerformImport(Uri);

            result.Outcome.Should().Be(ImportGpPracticeListOutcome.Success);
            result.ExtractDate.Should().Be(ExtractDate);
            result.TotalRecords.Should().Be(1);
            result.TotalRecordsUpdated.Should().Be(1);

            var actual = dbContext.GpPracticeSizes.First();

            actual.ExtractDate.Should().Be(CsvData.EXTRACT_DATE);
            actual.NumberOfPatients.Should().Be(CsvData.NUMBER_OF_PATIENTS);
            actual.OdsCode.Should().Be(CsvData.CODE);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ImportGpPracticeData_ProviderReturnsOlderRecordThanInDb_DoesNotImportRecordFromProvider(
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] IGpPracticeProvider mockProvider,
            GpPracticeImportService systemUnderTest)
        {
            mockProvider.GetGpPractices(Uri).Returns(new[] { CsvData });

            dbContext.GpPracticeSizes.Add(new GpPracticeSize
            {
                ExtractDate = new DateTime(2020, 1, 1),
                OdsCode = OdsCode,
                NumberOfPatients = 1,
            });

            dbContext.SaveChanges();

            var result = await systemUnderTest.PerformImport(Uri);

            result.Outcome.Should().Be(ImportGpPracticeListOutcome.Success);
            result.ExtractDate.Should().Be(ExtractDate);
            result.TotalRecords.Should().Be(1);
            result.TotalRecordsUpdated.Should().Be(1);

            var actual = dbContext.GpPracticeSizes.First();

            actual.ExtractDate.Should().Be(CsvData.EXTRACT_DATE);
            actual.NumberOfPatients.Should().Be(CsvData.NUMBER_OF_PATIENTS);
            actual.OdsCode.Should().Be(CsvData.CODE);
        }
    }
}
