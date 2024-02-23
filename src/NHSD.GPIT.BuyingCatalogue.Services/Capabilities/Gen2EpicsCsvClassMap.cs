using CsvHelper.Configuration;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.Services.Csv;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities;

public class Gen2EpicsCsvClassMap : ClassMap<Gen2EpicsCsvModel>
{
    public Gen2EpicsCsvClassMap()
    {
        Map(x => x.Key).Name("Key").TypeConverter<StripNewlineStringConverter>();
        Map(x => x.SupplierId).Name("Supplier ID").TypeConverter<StripNewlineStringConverter>();
        Map(x => x.SolutionId).Name("Solution ID").TypeConverter<StripNewlineStringConverter>();
        Map(x => x.AdditionalServiceId).Name("Additional Service ID").TypeConverter<StripNewlineStringConverter>();
        Map(x => x.CapabilityId).Name("Capability ID").TypeConverter<StripNewlineStringConverter>();
        Map(x => x.EpicId).Name("Epic ID").TypeConverter<StripNewlineStringConverter>();
        Map(x => x.EpicAssessmentResult).Name("Epic Final Assessment Result").TypeConverter<StripNewlineStringConverter>();
    }
}
