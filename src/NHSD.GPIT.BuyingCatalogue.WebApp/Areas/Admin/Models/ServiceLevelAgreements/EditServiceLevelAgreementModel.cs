using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements
{
    public class EditServiceLevelAgreementModel : NavBaseModel
    {
        public EditServiceLevelAgreementModel()
        {
            BackLinkText = "Go back";
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

        public TaskProgress Status()
        {
            // No SLA type has been added
            if (CatalogueItem.Solution.ServiceLevelAgreement is null)
                return TaskProgress.NotStarted;

            // Everything complete
            if (CatalogueItem.Solution.ServiceLevelAgreement is not null &&
                CatalogueItem.Solution.ServiceLevelAgreement.Contacts.Any() &&
                CatalogueItem.Solution.ServiceLevelAgreement.ServiceHours.Any() &&
                CatalogueItem.Solution.ServiceLevelAgreement.ServiceLevels.Any())
                return TaskProgress.Completed;

            // One or more sections not completed
            return TaskProgress.InProgress;
        }
    }
}
