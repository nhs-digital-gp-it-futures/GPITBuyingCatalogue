using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements
{
    public sealed class ServiceAvailabilityTimesModel
    {
        public string SupportType { get; set; }

        public DateTime From { get; set; }

        public DateTime Until { get; set; }

        public string ApplicableDays { get; set; }

        public int UserId { get; set; }
    }
}
