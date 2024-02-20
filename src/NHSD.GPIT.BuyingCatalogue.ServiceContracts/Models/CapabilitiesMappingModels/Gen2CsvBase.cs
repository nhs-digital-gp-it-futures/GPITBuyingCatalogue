namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

public abstract class Gen2CsvBase
{
    public string Key { get; set; }

    public string SupplierId { get; set; }

    public string SolutionId { get; set; }

    public string AdditionalServiceId { get; set; }

    public string CapabilityId { get; set; }
}
