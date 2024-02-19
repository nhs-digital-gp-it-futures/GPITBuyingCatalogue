using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilitiesMappingModels;

public class FailedCapabilitiesUploadModel
{
    public FailedCapabilitiesUploadModel()
    {
    }

    public FailedCapabilitiesUploadModel(
        string fileName,
        IEnumerable<Gen2CapabilitiesCsvModel> failedRecords)
    {
        FileName = fileName;
        FailedRecords = failedRecords;
    }

    public string FileName { get; set; }

    public IEnumerable<Gen2CapabilitiesCsvModel> FailedRecords { get; set; }
}
