namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements
{
    public class EditServiceLevelModel
    {
        public string ServiceType { get; set; }

        public string ServiceLevel { get; set; }

        public string HowMeasured { get; set; }

        public bool CreditsApplied { get; set; }
    }
}
