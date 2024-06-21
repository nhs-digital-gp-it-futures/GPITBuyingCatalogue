using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Email
{
    public static class ContactUsServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ContactUsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static Task SubmitQuery_NullOrEmptyFullName_ThrowsException(
            string fullName,
            string emailAddress,
            string message,
            ContactUsService service) => Assert.ThrowsAsync<ArgumentNullException>(() => service.SubmitQuery(fullName, emailAddress, message));

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static Task SubmitQuery_NullOrEmptyEmailAddress_ThrowsException(
            string emailAddress,
            string fullName,
            string message,
            ContactUsService service) => Assert.ThrowsAsync<ArgumentNullException>(() => service.SubmitQuery(fullName, emailAddress, message));

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        public static Task SubmitQuery_NullOrEmptyMessage_ThrowsException(
            string message,
            string fullName,
            string emailAddress,
            ContactUsService service) => Assert.ThrowsAsync<ArgumentNullException>(() => service.SubmitQuery(fullName, emailAddress, message));

        [Theory]
        [MockAutoData]
        public static async Task SubmitQuery_GeneralEnquiry_UsesCorrectEmail(
            string fullName,
            string emailAddress,
            string message,
            [Frozen] ContactUsSettings settings,
            [Frozen] IGovNotifyEmailService govNotifyEmailService,
            ContactUsService service)
        {
            await service.SubmitQuery(fullName, emailAddress, message);

            await govNotifyEmailService.Received().SendEmailAsync(settings.GeneralQueriesRecipient.Address, Arg.Any<string>(), Arg.Any<Dictionary<string, dynamic>>());
            await govNotifyEmailService.Received().SendEmailAsync(emailAddress, Arg.Any<string>(), null);
        }
    }
}
