using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public abstract class MarketingDisplayBaseModel
    {
        private readonly IList<SectionModel> sections = new List<SectionModel>
        {
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Description",
            },
            new()
            {
                Action = nameof(AboutSolutionController.Features),
                Controller = typeof(AboutSolutionController).ControllerName(),
                Name = "Features",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Capabilities",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "List price",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Additional Services",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Associated Services",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Interoperability",
            },
            new()
            {
                Action = nameof(AboutSolutionController.ImplementationTimescales),
                Controller = typeof(AboutSolutionController).ControllerName(),
                Name = "Implementation timescales",
            },
            new()
            {
                Action = nameof(ClientApplicationTypeController.ClientApplicationTypes),
                Controller = typeof(ClientApplicationTypeController).ControllerName(),
                Name = "Client application type",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Hosting type",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Service Level Agreement",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Development plans",
            },
            new()
            {
                Action = nameof(SolutionController.Index),
                Controller = typeof(SolutionController).ControllerName(),
                Name = "Supplier details",
            },
        };

        public abstract DateTime LastReviewed { get; set; }

        public abstract PaginationFooterModel PaginationFooter { get; set; }

        public abstract string Section { get; set; }

        public abstract string SolutionId { get; set; }

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
