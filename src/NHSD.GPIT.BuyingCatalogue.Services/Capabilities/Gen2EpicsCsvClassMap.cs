using CsvHelper.Configuration;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities;

public class Gen2EpicsCsvClassMap : ClassMap<Gen2EpicsCsvModel>
{
    public Gen2EpicsCsvClassMap()
    {
        Map(x => x.SupplierId).Name("Supplier ID");
        Map(x => x.SolutionId).Name("Solution ID");
        Map(x => x.AdditionalServiceId).Name("Additional Service ID");
        Map(x => x.CapabilityId).Name("Capability ID");
        Map(x => x.EpicId).Name("Epic ID");
        Map(x => x.EpicAssessmentResult).Name("Epic Final Assessment Result");
    }
}
