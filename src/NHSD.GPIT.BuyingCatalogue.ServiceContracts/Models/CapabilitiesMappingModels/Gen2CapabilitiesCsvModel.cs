using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

public class Gen2CapabilitiesCsvModel
{
    public static readonly string[] ValidAssessmentResults = ["Passed - Full", "Passed - Partial"];

    public string SupplierId { get; set; }

    public string SolutionId { get; set; }

    public string AdditionalServiceId { get; set; }

    public string CapabilityId { get; set; }

    public string CapabilityAssessmentResult { get; set; }
}
