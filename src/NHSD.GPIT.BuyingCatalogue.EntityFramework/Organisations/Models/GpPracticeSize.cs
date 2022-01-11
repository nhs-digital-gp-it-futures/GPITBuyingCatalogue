using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models
{
    public sealed class GpPracticeSize
    {
        public string OdsCode { get; set; }

        public int NumberOfPatients { get; set; }

        public DateTime ExtractDate { get; set; }
    }
}
