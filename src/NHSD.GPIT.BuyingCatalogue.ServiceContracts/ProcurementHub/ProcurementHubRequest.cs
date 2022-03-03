namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.ProcurementHub
{
    public class ProcurementHubRequest
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string OrganisationName { get; set; }

        public string OdsCode { get; set; }

        public string Query { get; set; }
    }
}
