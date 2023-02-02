using System.Collections.Generic;
using BuyingCatalogueFunction.Models.CapabilitiesUpdate.CsvModels;

namespace BuyingCatalogueFunction.Models.CapabilitiesUpdate;

public record CapabilitiesImportModel(List<CsvCapability> Capabilities, List<CsvStandard> Standards,
    List<CsvEpic> Epics, List<CsvRelationship> Relationships);
