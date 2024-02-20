using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

public class Gen2CsvImportModel<T>
    where T : Gen2CsvBase
{
    public Gen2CsvImportModel()
    {
    }

    public Gen2CsvImportModel(
        string fileName,
        IEnumerable<T> imported,
        IEnumerable<T> failed)
    {
        FileName = fileName;
        Imported = imported.ToList();
        Failed = failed.ToList();
    }

    public string FileName { get; set; }

    public ICollection<T> Imported { get; set; }

    public ICollection<T> Failed { get; set; }
}
