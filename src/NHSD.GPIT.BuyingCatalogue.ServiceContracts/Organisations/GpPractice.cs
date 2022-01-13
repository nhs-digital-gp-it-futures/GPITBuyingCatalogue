using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public sealed class GpPractice
    {
        public DateTime EXTRACT_DATE { get; set; }

        public string CODE { get; set; }

        public int NUMBER_OF_PATIENTS { get; set; }
    }
}
