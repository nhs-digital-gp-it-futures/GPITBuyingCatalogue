using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSideNavigationSection;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class PaginationFooterModel
    {
        public bool FullWidth { get; set; }

        public NhsSideNavigationSectionModel Next { get; set; }

        public NhsSideNavigationSectionModel Previous { get; set; }
    }
}
