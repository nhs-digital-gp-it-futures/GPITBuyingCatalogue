using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SolutionStandardsModel : SolutionDisplayBaseModel
    {
        public SolutionStandardsModel(
            CatalogueItem catalogueItem,
            IList<Standard> standards,
            IEnumerable<string> standardsWithWorkOffPlans,
            CatalogueItemContentStatus contentStatus)
            : base(catalogueItem, contentStatus)
        {
            Standards = standards;
            StandardsWithWorkOffPlans = standardsWithWorkOffPlans;
        }

        public SolutionStandardsModel()
        {
        }

        public override int Index => 11;

        public IList<Standard> Standards { get; init; }

        public IEnumerable<string> StandardsWithWorkOffPlans { get; init; }
    }
}
