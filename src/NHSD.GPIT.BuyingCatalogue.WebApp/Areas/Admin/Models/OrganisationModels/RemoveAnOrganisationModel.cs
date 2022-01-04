using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels
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
        }

        public Organisation CurrentOrganisation { get; init; }

        public Organisation RelatedOrganisation { get; init; }
    }
}
