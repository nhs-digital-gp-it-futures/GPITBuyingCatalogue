using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class CapabilitiesViewModel : SolutionDisplayBaseModel, INoNavModel
    {
        public CapabilitiesViewModel(CatalogueItem solution, CatalogueItemContentStatus contentStatus)
            : base(solution, contentStatus)
        {
            RowViewModels = solution.CatalogueItemCapabilities.Select(cic => new RowViewModel(cic)).ToList();
        }

        public CapabilitiesViewModel(CatalogueItem solution, CatalogueItem additionalService, CatalogueItemContentStatus contentStatus)
            : base(solution, contentStatus)
        {
            RowViewModels = additionalService.CatalogueItemCapabilities.Select(cic => new RowViewModel(cic)).ToList();
        }

        public CapabilitiesViewModel()
        {
        }

        public string CapabilitiesHeading { get; set; } = "Capabilities met";

        public override int Index => 3;

        public string Name { get; set; }

        public string Description { get; set; }

        public IList<RowViewModel> RowViewModels { get; } = new List<RowViewModel>();
    }
}
