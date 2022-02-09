using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Services.Email;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Notify.Interfaces;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Email
{
    public static class GovNotifyEmailServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(GovNotifyEmailService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task SendEmailAsync_ValidRequest_CallsNotificationClient(
            string emailAddress,
            string templateId,
            Dictionary<string, dynamic> personalisations,
            [Frozen] Mock<IAsyncNotificationClient> notificationClient,
            GovNotifyEmailService emailService)
        {
            await emailService.SendEmailAsync(emailAddress, templateId, personalisations);

            notificationClient.Verify(_ => _.SendEmailAsync(emailAddress, templateId, personalisations, null, null));
        }
    }
}
