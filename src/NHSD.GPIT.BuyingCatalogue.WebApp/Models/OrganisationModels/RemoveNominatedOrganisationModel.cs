namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels
{
    public class RemoveNominatedOrganisationModel : NavBaseModel
    {
        public int OrganisationId { get; init; }

        public string OrganisationName { get; init; }

        public int NominatedOrganisationId { get; init; }

        public string NominatedOrganisationName { get; init; }
    }
}
