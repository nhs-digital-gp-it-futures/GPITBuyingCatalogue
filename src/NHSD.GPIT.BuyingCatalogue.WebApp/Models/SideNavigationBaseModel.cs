using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    [ExcludeFromCodeCoverage]
    public abstract class SideNavigationBaseModel : NavBaseModel
    {
        private IList<SectionModel> sections = new List<SectionModel>();
        private IList<SectionModel> breadcrumbItems = new List<SectionModel>();

        public abstract int Index { get; }

        public bool ShowBreadcrumb => BreadcrumbItems?.Any() ?? false;

        public bool ShowSideNavigation { get; set; }

        public bool ShowPagination { get; set; }

        public bool ShowBackToTop { get; set; }

        public string FirstSection => sections.Any() ? sections[0].Name : null;

        public string SelectedSection => Index >= 0 && Index < sections.Count ? sections[Index].Name : null;

        public IList<SectionModel> BreadcrumbItems
        {
            get
            {
                return new List<SectionModel>(breadcrumbItems.Where(s => s.Show));
            }

            set
            {
                breadcrumbItems = value;
            }
        }

        public IList<SectionModel> Sections
        {
            get
            {
                var sectionsToShow = new List<SectionModel>(sections.Where(s => s.Show));

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
            var sectionsToShow = new List<SectionModel>(sections.Where(s => s.Show));
            if (sectionsToShow.FirstOrDefault(s => s.Name.EqualsIgnoreCase(SelectedSection)) is not { } sectionModel)
                return;

            var index = sectionsToShow.IndexOf(sectionModel);

            PaginationFooter.Previous = index > 0 ? sectionsToShow[index - 1] : null;
            PaginationFooter.Next = index < (sectionsToShow.Count - 1) ? sectionsToShow[index + 1] : null;
        }
    }
}
