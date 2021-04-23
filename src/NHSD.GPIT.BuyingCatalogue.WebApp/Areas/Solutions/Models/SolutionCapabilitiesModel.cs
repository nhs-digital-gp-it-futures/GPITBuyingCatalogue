using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionCapabilitiesModel 
    {        
        public SolutionCapabilitiesModel(SolutionCapability solutionCapability, Solution solution)
        {
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
            foreach(var solutionEpic in solution.SolutionEpics.Where(x=>x.CapabilityId == Id && x.Epic.Active && x.Epic.CompliancyLevel != null))
            {
                var epicLabel = $"{solutionEpic.Epic.Name} ({solutionEpic.Epic.Id})";

                if (solutionEpic.Epic.CompliancyLevel.Name == "MUST")
                {
                    if (solutionEpic.Status.IsMet)
                        MustEpicsMet.Add(epicLabel);
                    else
                        MustEpicsNotMet.Add(epicLabel);
                }

                else if (solutionEpic.Epic.CompliancyLevel.Name == "MAY")
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
