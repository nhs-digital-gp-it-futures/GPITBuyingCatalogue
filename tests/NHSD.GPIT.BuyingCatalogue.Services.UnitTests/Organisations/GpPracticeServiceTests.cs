using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Organisations
{
    public static class GpPracticeServiceTests
    {
        private const string EmailAddress = "a@b.com";

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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void ImportGpPracticeData_EmailAddressIsNull_ThrowsError(
            string emailAddress,
            GpPracticeService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.ImportGpPracticeData(Uri, emailAddress))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [CommonInlineAutoData(ImportGpPracticeListOutcome.CannotReadInputFile)]
        [CommonInlineAutoData(ImportGpPracticeListOutcome.WrongFormat)]
        public static async Task ImportGpPracticeData_ImportServiceReturnsError_SendsErrorEmail(
            ImportGpPracticeListOutcome outcome,
            [Frozen] ImportPracticeListMessageSettings settings,
            [Frozen] Mock<IGovNotifyEmailService> mockEmailService,
            [Frozen] Mock<IGpPracticeImportService> mockImportService,
            GpPracticeService systemUnderTest)
        {
            mockImportService
                .Setup(x => x.PerformImport(Uri))
                .ReturnsAsync(new ImportGpPracticeListResult
                {
                    Outcome = outcome,
                });

            mockEmailService
                .Setup(x => x.SendEmailAsync(EmailAddress, settings.ErrorTemplateId, null))
                .Verifiable();

            await systemUnderTest.ImportGpPracticeData(Uri, EmailAddress);

            mockImportService.VerifyAll();
            mockEmailService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static async Task ImportGpPracticeData_ImportServiceReturnsSuccess_SendsSuccessEmail(
            ImportGpPracticeListResult result,
            [Frozen] ImportPracticeListMessageSettings settings,
            [Frozen] Mock<IGovNotifyEmailService> mockEmailService,
            [Frozen] Mock<IGpPracticeImportService> mockImportService,
            GpPracticeService systemUnderTest)
        {
            result.Outcome = ImportGpPracticeListOutcome.Success;

            Dictionary<string, dynamic> tokens = null;

            mockImportService
                .Setup(x => x.PerformImport(Uri))
                .ReturnsAsync(result);

            mockEmailService
                .Setup(x => x.SendEmailAsync(EmailAddress, settings.SuccessTemplateId, It.IsAny<Dictionary<string, dynamic>>()))
                .Callback<string, string, Dictionary<string, dynamic>>((_, _, actualTokens) => tokens = actualTokens)
                .Returns(Task.CompletedTask);

            await systemUnderTest.ImportGpPracticeData(Uri, EmailAddress);

            mockImportService.VerifyAll();
            mockEmailService.VerifyAll();

            var extractDate = tokens.Should().ContainKey(GpPracticeService.ExtractDateToken).WhoseValue as string;
            var totalRecords = tokens.Should().ContainKey(GpPracticeService.TotalRecordsToken).WhoseValue as string;
            var totalUpdated = tokens.Should().ContainKey(GpPracticeService.TotalUpdatedToken).WhoseValue as string;

            extractDate.Should().Be($"{result.ExtractDate:dd MMMM yyyy}");
            totalRecords.Should().Be($"{result.TotalRecords}");
            totalUpdated.Should().Be($"{result.TotalRecordsUpdated}");
        }
    }
}
