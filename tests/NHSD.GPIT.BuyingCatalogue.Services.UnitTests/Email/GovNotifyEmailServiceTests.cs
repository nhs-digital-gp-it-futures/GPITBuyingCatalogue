using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Notify.Interfaces;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Email
{
    public static class GovNotifyEmailServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(GovNotifyEmailService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task SendEmailAsync_ValidRequest_CallsNotificationClient(
            string emailAddress,
            string templateId,
            Dictionary<string, dynamic> personalisations,
            [Frozen] IAsyncNotificationClient notificationClient,
            GovNotifyEmailService emailService)
        {
            await emailService.SendEmailAsync(emailAddress, templateId, personalisations);

            await notificationClient.Received().SendEmailAsync(emailAddress, templateId, personalisations, null, null);
        }
    }
}
