using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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
                Action = nameof(SolutionDetailsController.Description),
                Controller = ControllerName,
                Name = "Features",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
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
                Action = nameof(SolutionDetailsController.Description),
                Controller = ControllerName,
                Name = "Implementation timescales",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
                Controller = ControllerName,
                Name = "Client application type",
            },
            new()
            {
                Action = nameof(SolutionDetailsController.Description),
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

        public string LastReviewed { get; set; }

        public PaginationFooterModel PaginationFooter { get; set; }

        public abstract string Section { get; set; }

        public string SolutionId { get; set; }

        public string SolutionName { get; set; }

        public SectionModel GetSectionFor(string section) =>
            sections.FirstOrDefault(s => s.Name.EqualsIgnoreCase(section)) is { } sectionModel
                ? sectionModel
                : null;

        public virtual IList<SectionModel> GetSections()
        {
            var sectionModels = new List<SectionModel>(sections);
            sectionModels.ForEach(s => s.Id = SolutionId);

            if (GetSectionFor(Section) is { } sectionModel)
                sectionModel.Selected = true;

            return sectionModels;
        }
    }
}
