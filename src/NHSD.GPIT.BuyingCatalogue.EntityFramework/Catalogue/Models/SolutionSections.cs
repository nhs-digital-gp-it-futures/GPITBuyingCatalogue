using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

[Serializable]
public sealed class SolutionSections
{
    public SolutionType SolutionType { get; set; }

    public SolutionSectionRequirement Details { get; set; }

    public SolutionSectionRequirement Description { get; set; }

    public SolutionSectionRequirement Features { get; set; }

    public SolutionSectionRequirement Integrations { get; set; }

    public SolutionSectionRequirement Implementation { get; set; }

    public SolutionSectionRequirement ApplicationType { get; set; }

    public SolutionSectionRequirement HostingType { get; set; }

    public SolutionSectionRequirement DataProcessingInformation { get; set; }

    public SolutionSectionRequirement ListPrice { get; set; }

    public SolutionSectionRequirement CapabilitiesAndEpics { get; set; }

    public SolutionSectionRequirement AdditionalServices { get; set; }

    public SolutionSectionRequirement AssociatedServices { get; set; }

    public SolutionSectionRequirement Standards { get; set; }

    public SolutionSectionRequirement DevelopmentPlans { get; set; }

    public SolutionSectionRequirement SupplierDetails { get; set; }

    public SolutionSectionRequirement ServiceLevelAgreement { get; set; }
}
