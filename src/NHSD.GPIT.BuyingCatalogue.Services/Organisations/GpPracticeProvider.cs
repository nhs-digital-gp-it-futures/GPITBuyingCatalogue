using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Streaming;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public class GpPracticeProvider : IGpPracticeProvider
    {
        private readonly ILogWrapper<GpPracticeProvider> logger;
        private readonly IStreamingService streamingService;

        public GpPracticeProvider(
            ILogWrapper<GpPracticeProvider> logger,
            IStreamingService streamingService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.streamingService = streamingService ?? throw new ArgumentNullException(nameof(streamingService));
        }

        public async Task<IEnumerable<GpPractice>> GetGpPractices(Uri csvUri)
        {
            try
            {
                await using var stream = await streamingService.StreamContents(csvUri);

                if (stream == null)
                    return null;

                using var reader = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);

                return reader.GetRecords<GpPractice>().ToList();
            }
            catch (HeaderValidationException e)
            {
                logger.LogError(e, "Failed to read gp practice records");
                throw new FormatException("Input file is not in the correct format", e);
            }
        }
    }
}
