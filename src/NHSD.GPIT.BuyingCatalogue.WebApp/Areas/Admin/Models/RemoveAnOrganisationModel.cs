using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class RemoveAnOrganisationModel : NavBaseModel
    {
        public RemoveAnOrganisationModel()
        {
        }

        public RemoveAnOrganisationModel(Organisation currentOrganisation, Organisation relatedOrganisation)
        {
            CurrentOrganisation = currentOrganisation;
            RelatedOrganisation = relatedOrganisation;
            BackLink = $"/admin/organisations/{currentOrganisation.Id}";
        }

        public Organisation CurrentOrganisation { get; init; }

        public Organisation RelatedOrganisation { get; init; }
    }
}
