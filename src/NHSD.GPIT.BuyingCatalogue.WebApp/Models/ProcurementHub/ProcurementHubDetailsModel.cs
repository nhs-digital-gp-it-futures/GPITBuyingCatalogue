namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.ProcurementHub
{
    public class ProcurementHubDetailsModel : NavBaseModel
    {
        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public string OrganisationName { get; set; }

        public string OdsCode { get; set; }

        public string Query { get; set; }

        public bool HasReadPrivacyPolicy { get; set; }
    }
}
