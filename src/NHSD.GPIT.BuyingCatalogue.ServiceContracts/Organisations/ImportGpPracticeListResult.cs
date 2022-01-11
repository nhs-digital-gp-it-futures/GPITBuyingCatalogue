using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public class ImportGpPracticeListResult
    {
        public ImportGpPracticeListOutcome Outcome { get; set; }

        public int TotalRecords { get; set; }

        public int TotalRecordsImported { get; set; }

        public long TimeTaken { get; set; }

        public override string ToString()
        {
            return "Import GP Practice List Result:" + Environment.NewLine
                + $"Outcome: {Outcome}" + Environment.NewLine
                + $"Total Records: {TotalRecords}" + Environment.NewLine
                + $"Total Records Imported: {TotalRecordsImported}" + Environment.NewLine
                + $"Time Taken: {TimeTaken}";
        }
    }
}
