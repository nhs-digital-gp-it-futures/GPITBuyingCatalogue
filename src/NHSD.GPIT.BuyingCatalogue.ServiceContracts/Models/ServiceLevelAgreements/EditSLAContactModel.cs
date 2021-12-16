using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements
{
    public sealed class EditSLAContactModel
    {
        public int Id { get; set; }

        public string SolutionId { get; set; }

        public string Channel { get; set; }

        public string ContactInformation { get; set; }

        public string ApplicableDays { get; set; }

        public DateTime TimeFrom { get; set; }

        public DateTime TimeUntil { get; set; }

        public int UserId { get; set; }
    }
}
