namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

public class Gen2CapabilitiesCsvModel : Gen2CsvBase
{
    public static readonly string[] ValidAssessmentResults = new[] { "Passed - Full", "Passed - Partial" };

    public string CapabilityAssessmentResult { get; set; }
}
