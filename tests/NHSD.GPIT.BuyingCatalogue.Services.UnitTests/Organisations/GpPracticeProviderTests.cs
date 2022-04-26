using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Streaming;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Organisations
{
    public static class GpPracticeProviderTests
    {
        private const string OdsCode = "A00001";
        private const int NumberOfPatients = 1234;
        private static readonly DateTime ExtractDate = new(2000, 1, 1);

        private static readonly string CsvContents = "EXTRACT_DATE,CODE,NUMBER_OF_PATIENTS"
            + Environment.NewLine
            + $"{ExtractDate:ddMMMyyyy},{OdsCode},{NumberOfPatients}";

        private static readonly Uri Uri = new("http://www.test.com");

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(GpPracticeProvider).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetGpPractices_StreamingServiceReturnsNull_ReturnsNull(
            [Frozen] Mock<IStreamingService> mockStreamingService,
            GpPracticeProvider systemUnderTest)
        {
            mockStreamingService
                .Setup(x => x.StreamContents(Uri))
                .ReturnsAsync((Stream)null);

            var result = await systemUnderTest.GetGpPractices(Uri);

            result.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetGpPractices_StreamingServiceReturnsValidRecord_ReturnsRecord(
            [Frozen] Mock<IStreamingService> mockStreamingService,
            GpPracticeProvider systemUnderTest)
        {
            mockStreamingService
                .Setup(x => x.StreamContents(Uri))
                .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(CsvContents)));

            var result = (await systemUnderTest.GetGpPractices(Uri)).ToList();

            result.Should().ContainSingle();
            result.First().Should().BeEquivalentTo(new GpPractice
            {
                CODE = OdsCode,
                EXTRACT_DATE = ExtractDate,
                NUMBER_OF_PATIENTS = NumberOfPatients,
            });
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void GetGpPractices_StreamingServiceReturnsUnrecognisedFormat_ThrowsException(
            [Frozen] Mock<IStreamingService> mockStreamingService,
            GpPracticeProvider systemUnderTest)
        {
            var csvContents = "UNRECOGNISED_HEADER" + Environment.NewLine + "value";

            mockStreamingService
                .Setup(x => x.StreamContents(Uri))
                .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(csvContents)));

            FluentActions
                .Awaiting(() => systemUnderTest.GetGpPractices(Uri))
                .Should().ThrowAsync<FormatException>();
        }
    }
}
