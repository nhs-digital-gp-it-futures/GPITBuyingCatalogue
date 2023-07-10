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
            SupplierName = solution.Supplier.Name;
            IsFoundation = solution.Solution.FrameworkSolutions.Any(fs => fs.IsFoundation).ToYesNo();
        }

        public CapabilitiesViewModel(CatalogueItem solution, CatalogueItem additionalService, CatalogueItemContentStatus contentStatus)
            : base(solution, contentStatus)
        {
            RowViewModels = additionalService.CatalogueItemCapabilities.Select(cic => new RowViewModel(cic)).ToList();
        }

        public CapabilitiesViewModel()
        {
        }

        public string SupplierName { get; }

        public string IsFoundation { get; }

        public string CapabilitiesHeading { get; set; } = "Capabilities met";

        public override int Index => 3;

        public string Name { get; set; }

        public string Description { get; set; }

        public string BackLinkText { get; set; }

        public string BackLink { get; set; }

        public IList<RowViewModel> RowViewModels { get; } = new List<RowViewModel>();

        public string FrameworkTitle() => Frameworks is not null && Frameworks.Any() && Frameworks.Count > 1
            ? "Frameworks"
            : "Framework";
    }
}
