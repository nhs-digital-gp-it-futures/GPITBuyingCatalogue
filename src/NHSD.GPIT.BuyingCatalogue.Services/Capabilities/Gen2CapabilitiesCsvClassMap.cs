using CsvHelper.Configuration;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities;

public class Gen2CapabilitiesCsvClassMap : ClassMap<Gen2CapabilitiesCsvModel>
{
    public Gen2CapabilitiesCsvClassMap()
    {
        Map(x => x.SupplierId).Name("Supplier ID");
        Map(x => x.SolutionId).Name("Solution ID");
        Map(x => x.AdditionalServiceId).Name("Additional Service ID");
        Map(x => x.CapabilityId).Name("Capability ID");
        Map(x => x.CapabilityAssessmentResult).Name("Capability Assessment Result");
    }
}
