using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilitiesMappingModels;

public class FailedGen2UploadModel<T>
    where T : Gen2CsvBase
{
    public FailedGen2UploadModel()
    {
    }

    public FailedGen2UploadModel(
        IEnumerable<T> failedRecords)
    {
        FailedRecords = failedRecords;
    }

    public IEnumerable<T> FailedRecords { get; set; }
}
