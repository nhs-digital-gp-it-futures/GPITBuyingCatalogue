using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SolutionStandardsModel : SolutionDisplayBaseModel
    {
        public SolutionStandardsModel(CatalogueItem catalogueItem, IList<Standard> standards, IEnumerable<string> standardsWithWorkOffPlans)
            : base(catalogueItem)
        {
            Standards = standards;
            StandardsWithWorkOffPlans = standardsWithWorkOffPlans;
        }

        public SolutionStandardsModel()
        {
        }

        public override int Index => 3;

        public IList<Standard> Standards { get; init; }

        public IEnumerable<string> StandardsWithWorkOffPlans { get; init; }
    }
}
