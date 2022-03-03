using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
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
        [CommonAutoData]
        public static async Task ImportGpPracticeData_ImportServiceReturnsResult_EnqueuesEmail(
            ImportGpPracticeListResult result,
            [Frozen] Mock<IBackgroundJobClient> mockClient,
            [Frozen] Mock<IGpPracticeImportService> mockImportService,
            GpPracticeService systemUnderTest)
        {
            Job job = null;

            mockImportService
                .Setup(x => x.PerformImport(Uri))
                .ReturnsAsync(result);

            mockClient
                .Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                .Callback<Job, IState>((j, _) => job = j)
                .Returns(string.Empty);

            await systemUnderTest.ImportGpPracticeData(Uri, EmailAddress);

            mockImportService.VerifyAll();
            mockClient.VerifyAll();

            job.Type.Should().BeAssignableTo<IGpPracticeService>();
            job.Method.Name.Should().Be(nameof(IGpPracticeService.SendConfirmationEmail));
            job.Args[0].Should().Be(result);
            job.Args[1].Should().Be(EmailAddress);
        }

        [Theory]
        [CommonAutoData]
        public static void SendConfirmationEmail_ResultIsNull_ThrowsException(
            GpPracticeService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.SendConfirmationEmail(null, EmailAddress))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("result");
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void SendConfirmationEmail_EmailAddressIsNull_ThrowsException(
            string emailAddress,
            ImportGpPracticeListResult result,
            GpPracticeService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.SendConfirmationEmail(result, emailAddress))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("emailAddress");
        }

        [Theory]
        [CommonInlineAutoData(ImportGpPracticeListOutcome.CannotReadInputFile)]
        [CommonInlineAutoData(ImportGpPracticeListOutcome.WrongFormat)]
        public static async Task SendConfirmationEmail_ResultIsInError_SendsErrorEmail(
            ImportGpPracticeListOutcome outcome,
            ImportGpPracticeListResult result,
            [Frozen] ImportPracticeListMessageSettings settings,
            [Frozen] Mock<IGovNotifyEmailService> mockEmailService,
            GpPracticeService systemUnderTest)
        {
            result.Outcome = outcome;

            mockEmailService
                .Setup(x => x.SendEmailAsync(EmailAddress, settings.ErrorTemplateId, null))
                .Verifiable();

            await systemUnderTest.SendConfirmationEmail(result, EmailAddress);

            mockEmailService.VerifyAll();
        }

        [Theory]
        [CommonAutoData]
        public static async Task SendConfirmationEmail_ResultIsSuccess_SendsSuccessEmail(
            ImportGpPracticeListResult result,
            [Frozen] ImportPracticeListMessageSettings settings,
            [Frozen] Mock<IGovNotifyEmailService> mockEmailService,
            GpPracticeService systemUnderTest)
        {
            result.Outcome = ImportGpPracticeListOutcome.Success;

            Dictionary<string, dynamic> tokens = null;

            mockEmailService
                .Setup(x => x.SendEmailAsync(EmailAddress, settings.SuccessTemplateId, It.IsAny<Dictionary<string, dynamic>>()))
                .Callback<string, string, Dictionary<string, dynamic>>((_, _, actualTokens) => tokens = actualTokens)
                .Returns(Task.CompletedTask);

            await systemUnderTest.SendConfirmationEmail(result, EmailAddress);

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
