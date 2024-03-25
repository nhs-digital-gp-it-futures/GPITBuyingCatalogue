namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

public class Gen2CapabilitiesCsvModel : Gen2CsvBase
{
    public string CapabilityAssessmentResult { get; set; }

    public bool IsValidAssessmentResult => CapabilityAssessmentResult.StartsWith("Passed");
}
