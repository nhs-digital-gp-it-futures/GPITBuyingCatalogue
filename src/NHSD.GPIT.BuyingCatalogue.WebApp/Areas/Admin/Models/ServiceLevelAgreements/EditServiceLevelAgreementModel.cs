using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements
{
    public class EditServiceLevelAgreementModel : NavBaseModel
    {
        public EditServiceLevelAgreementModel()
        {
        }

        public EditServiceLevelAgreementModel(CatalogueItem solution)
        {
            CatalogueItem = solution ?? throw new ArgumentNullException(nameof(solution));
        }

        public EditServiceLevelAgreementModel(CatalogueItem solution, EntityFramework.Catalogue.Models.ServiceLevelAgreements serviceLevelAgreements)
            : this(solution)
        {
            ServiceLevelAgreements = serviceLevelAgreements ?? throw new ArgumentNullException(nameof(serviceLevelAgreements));
        }

        public CatalogueItem CatalogueItem { get; init; }

        public EntityFramework.Catalogue.Models.ServiceLevelAgreements ServiceLevelAgreements { get; init; }
    }
}
