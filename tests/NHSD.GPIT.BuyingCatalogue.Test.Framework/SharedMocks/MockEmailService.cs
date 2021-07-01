using System.Threading.Tasks;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.SharedMocks
{
    public sealed class MockEmailService : IEmailService
    {
        private readonly IEmailService service;

        public MockEmailService()
        {
            var mockService = new Mock<IEmailService>();
            mockService
                .Setup(s => s.SendEmailAsync(It.IsNotNull<EmailMessage>()))
                .Callback<EmailMessage>(m => SentMessage = m);

            service = mockService.Object;
        }

        public EmailMessage SentMessage { get; private set; }

        public Task SendEmailAsync(EmailMessage emailMessage)
        {
            return service.SendEmailAsync(emailMessage);
        }
    }
}
