using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class FoundationCapabilitiesModel
    {
        private const int AppointmentsManagementGP = 5;
        private const int ReferralManagement = 11;
        private const int ResourceManagement = 12;
        private const int PatientInformationMaintenance = 13;
        private const int Prescribing = 14;
        private const int RecordingConsultations = 15;

        private readonly Dictionary<int, string[]> capabilityIds = new Dictionary<int, string[]>
        {
            { AppointmentsManagementGP, Array.Empty<string>() },
            { ReferralManagement, Array.Empty<string>() },
            { ResourceManagement, Array.Empty<string>() },
            { PatientInformationMaintenance, Array.Empty<string>() },
            { Prescribing, Array.Empty<string>() },
            { RecordingConsultations, Array.Empty<string>() },
        };

        public string ToFilterString() => capabilityIds.ToFilterString();
    }
}
