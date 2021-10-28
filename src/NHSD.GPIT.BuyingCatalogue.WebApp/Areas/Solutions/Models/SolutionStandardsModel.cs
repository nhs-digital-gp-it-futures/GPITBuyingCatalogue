using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SolutionStandardsModel : SolutionDisplayBaseModel
    {
        public SolutionStandardsModel(CatalogueItem catalogueItem, IList<Standard> standards)
            : base(catalogueItem)
        {
            Standards = standards;
        }

        public SolutionStandardsModel()
        {
        }

        public override int Index => 3;

        public IList<Standard> Standards { get; init; }
    }
}
