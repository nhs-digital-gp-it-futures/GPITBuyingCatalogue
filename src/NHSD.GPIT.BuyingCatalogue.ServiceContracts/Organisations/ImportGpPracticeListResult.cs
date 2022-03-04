using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public class ImportGpPracticeListResult
    {
        public ImportGpPracticeListOutcome Outcome { get; set; }

        public int TotalRecords { get; set; }

        public int TotalRecordsUpdated { get; set; }

        public DateTime? ExtractDate { get; set; }
    }
}
