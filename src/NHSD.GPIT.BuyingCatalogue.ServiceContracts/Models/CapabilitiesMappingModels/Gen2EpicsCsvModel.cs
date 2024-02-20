namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

public class Gen2EpicsCsvModel : Gen2CsvBase
{
    public static readonly string[] ValidAssessmentResults = ["Passed"];

    public string EpicId { get; set; }

    public string EpicAssessmentResult { get; set; }
}
