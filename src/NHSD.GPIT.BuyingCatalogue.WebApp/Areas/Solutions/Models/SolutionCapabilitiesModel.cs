using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SolutionCapabilitiesModel
    {
        public SolutionCapabilitiesModel(CatalogueItemCapability solutionCapability, Solution solution)
        {
            if (solutionCapability is null)
                throw new ArgumentNullException(nameof(solutionCapability));

            if (solution is null)
                throw new ArgumentNullException(nameof(solution));

            Id = solutionCapability.Capability.Id;
            Name = $"{solutionCapability.Capability.Name}, {solutionCapability.Capability.Version}";
            SourceUrl = solutionCapability.Capability.SourceUrl;
            Description = solutionCapability.Capability.Description;
            PopulateEpics(solution);
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string SourceUrl { get; set; }

        public string Description { get; set; }

        public List<string> MustEpicsMet { get; set; } = new();

        public List<string> MustEpicsNotMet { get; set; } = new();

        public List<string> MayEpicsMet { get; set; } = new();

        public List<string> MayEpicsNotMet { get; set; } = new();

        private void PopulateEpics(Solution solution)
        {
            foreach (var solutionEpic in solution.SolutionEpics.Where(se => se.CapabilityId == Id && se.Epic.Active))
            {
                var epicLabel = $"{solutionEpic.Epic.Name} ({solutionEpic.Epic.Id})";

                if (solutionEpic.Epic.CompliancyLevel == CompliancyLevel.Must)
                {
                    if (solutionEpic.Status.IsMet)
                        MustEpicsMet.Add(epicLabel);
                    else
                        MustEpicsNotMet.Add(epicLabel);
                }
                else if (solutionEpic.Epic.CompliancyLevel == CompliancyLevel.May)
                {
                    if (solutionEpic.Status.IsMet)
                        MayEpicsMet.Add(epicLabel);
                    else
                        MayEpicsNotMet.Add(epicLabel);
                }
            }
        }
    }
}
