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
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(GpPracticeService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void ImportGpPracticeData_UriIsNull_ThrowsError(
            GpPracticeService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.ImportGpPracticeData(null, EmailAddress))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void ImportGpPracticeData_EmailAddressIsNull_ThrowsError(
            string emailAddress,
            GpPracticeService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.ImportGpPracticeData(Uri, emailAddress))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockAutoData]
        public static void SendConfirmationEmail_ResultIsNull_ThrowsException(
            GpPracticeService systemUnderTest)
        {
            FluentActions
                .Awaiting(() => systemUnderTest.SendConfirmationEmail(null, EmailAddress))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("result");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
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
        [MockInlineAutoData(ImportGpPracticeListOutcome.CannotReadInputFile)]
        [MockInlineAutoData(ImportGpPracticeListOutcome.WrongFormat)]
        public static async Task SendConfirmationEmail_ResultIsInError_SendsErrorEmail(
            ImportGpPracticeListOutcome outcome,
            ImportGpPracticeListResult result,
            [Frozen] ImportPracticeListMessageSettings settings,
            [Frozen] IGovNotifyEmailService mockEmailService,
            GpPracticeService systemUnderTest)
        {
            result.Outcome = outcome;

            await systemUnderTest.SendConfirmationEmail(result, EmailAddress);

            await mockEmailService.Received().SendEmailAsync(EmailAddress, settings.ErrorTemplateId, null);
        }

        [Theory]
        [MockAutoData]
        public static async Task SendConfirmationEmail_ResultIsSuccess_SendsSuccessEmail(
            ImportGpPracticeListResult result,
            [Frozen] ImportPracticeListMessageSettings settings,
            [Frozen] IGovNotifyEmailService mockEmailService,
            GpPracticeService systemUnderTest)
        {
            result.Outcome = ImportGpPracticeListOutcome.Success;

            Dictionary<string, dynamic> tokens = null;

            mockEmailService.SendEmailAsync(EmailAddress, settings.SuccessTemplateId, Arg.Do<Dictionary<string, dynamic>>(actualTokens => tokens = actualTokens)).Returns(Task.CompletedTask);

            await systemUnderTest.SendConfirmationEmail(result, EmailAddress);

            await mockEmailService.Received().SendEmailAsync(EmailAddress, settings.SuccessTemplateId, Arg.Any<Dictionary<string, dynamic>>());

            var extractDate = tokens.Should().ContainKey(GpPracticeService.ExtractDateToken).WhoseValue as string;
            var totalRecords = tokens.Should().ContainKey(GpPracticeService.TotalRecordsToken).WhoseValue as string;
            var totalUpdated = tokens.Should().ContainKey(GpPracticeService.TotalUpdatedToken).WhoseValue as string;

            extractDate.Should().Be($"{result.ExtractDate:dd MMMM yyyy}");
            totalRecords.Should().Be($"{result.TotalRecords}");
            totalUpdated.Should().Be($"{result.TotalRecordsUpdated}");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetNumberOfPatients_ValueExists_ReturnsValue(
            [Frozen] BuyingCatalogueDbContext dbContext,
            GpPracticeService systemUnderTest)
        {
            dbContext.GpPracticeSizes.Add(GpPracticeSize);
            dbContext.SaveChanges();

            var result = await systemUnderTest.GetNumberOfPatients(new[] { OdsCode });

            result.Should().ContainSingle();
            result.First().Should().BeEquivalentTo(GpPracticeSize);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetNumberOfPatients_ValueDoesNotExist_ReturnsNull(
            GpPracticeService systemUnderTest)
        {
            var result = await systemUnderTest.GetNumberOfPatients(new[] { OdsCode });

            result.Should().BeEmpty();
        }
    }
}
