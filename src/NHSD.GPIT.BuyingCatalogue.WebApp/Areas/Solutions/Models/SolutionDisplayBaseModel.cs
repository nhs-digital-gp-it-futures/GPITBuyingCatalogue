using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public abstract class SolutionDisplayBaseModel
    {
        private static readonly string ControllerName = typeof(SolutionDetailsController).ControllerName();
        private readonly IList<SectionModel> sections = new List<SectionModel>
        {
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = ControllerName,
                Name = "Description",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Features),
                Controller = ControllerName,
                Name = "Features",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Capabilities),
                Controller = ControllerName,
                Name = "Capabilities",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = ControllerName,
                Name = "List price",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = ControllerName,
                Name = "Additional Services",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = ControllerName,
                Name = "Associated Services",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = ControllerName,
                Name = "Interoperability",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.ImplementationTimescales),
                Controller = ControllerName,
                Name = "Implementation timescales",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.ClientApplicationTypes),
                Controller = ControllerName,
                Name = "Client application type",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.HostingType),
                Controller = ControllerName,
                Name = "Hosting type",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = ControllerName,
                Name = "Service Level Agreement",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = ControllerName,
                Name = "Development plans",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = ControllerName,
                Name = "Supplier details",
            },
        };

        public ClientApplication ClientApplication { get; set; }

        public abstract int Index { get; }

        public string LastReviewed { get; set; }

        public PaginationFooterModel PaginationFooter { get; set; } = new();

        public string Section => sections[Index].Name;

        public string SolutionId { get; set; }

        public string SolutionName { get; set; }

        public virtual IList<SectionModel> GetSections()
        {
            var sectionsToShow = new List<SectionModel>(sections.Where(s => s.Show));
            sectionsToShow.ForEach(s => s.Id = SolutionId);

            if (sectionsToShow.FirstOrDefault(s => s.Name.EqualsIgnoreCase(Section)) is { } sectionModel)
                sectionModel.Selected = true;

            return sectionsToShow;
        }

        public void SetPaginationFooter()
        {
            var sectionsToShow = new List<SectionModel>(sections.Where(s => s.Show));
            if (sectionsToShow.FirstOrDefault(s => s.Name.EqualsIgnoreCase(Section)) is not { } sectionModel)
                return;

            var index = sectionsToShow.IndexOf(sectionModel);

            PaginationFooter.Previous = index > 0 ? sectionsToShow[index - 1] : null;
            PaginationFooter.Next = index < (sectionsToShow.Count - 1) ? sectionsToShow[index + 1] : null;
        }

        public void SetShowTrue(int index) => sections[index].Show = true;
    }
}
