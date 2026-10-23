using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SolutionStandardsModel : SolutionDisplayBaseModel, IStandardsModel
    {
        public SolutionStandardsModel(
            CatalogueItem catalogueItem,
            IEnumerable<StandardComplianceModel> standards,
            IEnumerable<string> standardsWithWorkOffPlans,
            CatalogueItemContentStatus contentStatus)
            : base(catalogueItem, contentStatus, true, false)
        {
            Standards = standards
                .Where(x => x.Compliance is StandardCompliance.InProgress or StandardCompliance.FullyMet)
                .ToList();

            StandardsWithWorkOffPlans = standardsWithWorkOffPlans;
        }

        public SolutionStandardsModel()
        {
        }

        public override int Index => 12;

        public IList<StandardComplianceModel> Standards { get; init; }

        public IEnumerable<string> StandardsWithWorkOffPlans { get; init; }
    }
}
