using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSideNavigationSection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    [ExcludeFromCodeCoverage]
    public abstract class SideNavigationBaseModel : NavBaseModel
    {
        private IList<NhsSideNavigationSectionModel> sections = new List<NhsSideNavigationSectionModel>();
        private IList<NhsSideNavigationSectionModel> breadcrumbItems = new List<NhsSideNavigationSectionModel>();

        public abstract int Index { get; }

        public bool ShowBreadcrumb => BreadcrumbItems?.Any() ?? false;

        public bool ShowSideNavigation { get; set; }

        public bool ShowPagination { get; set; }

        public bool ShowBackToTop { get; set; }

        public string FirstSection => sections[0].Name;

        public string SelectedSection => sections[Index].Name;

        public IList<NhsSideNavigationSectionModel> BreadcrumbItems
        {
            get
            {
                return new List<NhsSideNavigationSectionModel>(breadcrumbItems.Where(s => s.Show));
            }

            set
            {
                breadcrumbItems = value;
            }
        }

        public IList<NhsSideNavigationSectionModel> Sections
        {
            get
            {
                var sectionsToShow = new List<NhsSideNavigationSectionModel>(sections.Where(s => s.Show));

                if (sectionsToShow.FirstOrDefault(s => s.Name.EqualsIgnoreCase(SelectedSection)) is { } sectionModel)
                    sectionModel.Selected = true;

                return sectionsToShow;
            }

            set
            {
                sections = value;
            }
        }

        public bool NotFirstSection => !SelectedSection.EqualsIgnoreCase(FirstSection);

        public PaginationFooterModel PaginationFooter { get; set; } = new();

        public void SetPaginationFooter()
        {
            var sectionsToShow = new List<NhsSideNavigationSectionModel>(sections.Where(s => s.Show));
            if (sectionsToShow.FirstOrDefault(s => s.Name.EqualsIgnoreCase(SelectedSection)) is not { } sectionModel)
                return;

            var index = sectionsToShow.IndexOf(sectionModel);

            PaginationFooter.Previous = index > 0 ? sectionsToShow[index - 1] : null;
            PaginationFooter.Next = index < (sectionsToShow.Count - 1) ? sectionsToShow[index + 1] : null;
        }
    }
}
