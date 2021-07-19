namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    public interface IOdsSettings
    {
        public string ApiBaseUrl { get; set; }

        public string[] BuyerOrganisationRoleIds { get; set; }

        public string GpPracticeRoleId { get; set; }

        public int GetChildOrganisationSearchLimit { get; set; }
    }
}
