using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public sealed class GpPracticeService : IGpPracticeService
    {
        public const string ExtractDateToken = "extract_date";
        public const string TotalRecordsToken = "total_records";
        public const string TotalUpdatedToken = "total_updated";

        private readonly ImportPracticeListMessageSettings settings;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IGovNotifyEmailService emailService;
        private readonly IGpPracticeImportService gpPracticeImportService;

        public GpPracticeService(
            ImportPracticeListMessageSettings settings,
            IBackgroundJobClient backgroundJobClient,
            IGovNotifyEmailService emailService,
            IGpPracticeImportService gpPracticeImportService)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.backgroundJobClient = backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.gpPracticeImportService = gpPracticeImportService ?? throw new ArgumentNullException(nameof(gpPracticeImportService));
        }

        public async Task ImportGpPracticeData(Uri csvUri, string emailAddress)
        {
            if (csvUri == null)
                throw new ArgumentNullException(nameof(csvUri));

            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress));

            var result = await gpPracticeImportService.PerformImport(csvUri);

            backgroundJobClient.Enqueue(() => SendConfirmationEmail(result, emailAddress));
        }

        public async Task SendConfirmationEmail(ImportGpPracticeListResult result, string emailAddress)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress));

            var tokens = new Dictionary<string, dynamic>
            {
                { ExtractDateToken, $"{result.ExtractDate:dd MMMM yyyy}" },
                { TotalRecordsToken, $"{result.TotalRecords}" },
                { TotalUpdatedToken, $"{result.TotalRecordsUpdated}" },
            };

            switch (result.Outcome)
            {
                case ImportGpPracticeListOutcome.Success:
                    await emailService.SendEmailAsync(emailAddress, settings.SuccessTemplateId, tokens);
                    break;

                case ImportGpPracticeListOutcome.CannotReadInputFile:
                case ImportGpPracticeListOutcome.WrongFormat:
                    await emailService.SendEmailAsync(emailAddress, settings.ErrorTemplateId, null);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(result));
            }
        }
    }
}
