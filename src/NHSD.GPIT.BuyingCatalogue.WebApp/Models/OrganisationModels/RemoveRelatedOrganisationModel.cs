namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels
{
    public class RemoveRelatedOrganisationModel : NavBaseModel
    {
        public int OrganisationId { get; init; }

        public string OrganisationName { get; init; }

        public int RelatedOrganisationId { get; init; }

        public string RelatedOrganisationName { get; init; }

        public string ControllerName { get; set; }
    }
}
