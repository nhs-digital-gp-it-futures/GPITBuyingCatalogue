namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity
{
    public class NewAccountDetails
    {
        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public string OrganisationName { get; set; }

        public string OdsCode { get; set; }

        public bool HasGivenUserResearchConsent { get; set; }
    }
}
