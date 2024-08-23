using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements
{
    public sealed class ServiceAvailabilityTimesModel
    {
        public string SupportType { get; set; }

        public DateTime From { get; set; }

        public DateTime Until { get; set; }

        public ICollection<Iso8601DayOfWeek> ApplicableDays { get; set; }

        public bool IncludesBankHolidays { get; set; }

        public string AdditionalInformation { get; set; }

        public int UserId { get; set; }
    }
}
