using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;

public interface IGen2UploadService
{
    Task<Gen2CsvImportModel<Gen2CapabilitiesCsvModel>> GetCapabilitiesFromCsv(Stream capabilitiesStream);

    Task<Gen2CsvImportModel<Gen2EpicsCsvModel>> GetEpicsFromCsv(Stream epicsStream);

    Task<Stream> WriteToCsv(IEnumerable<Gen2CapabilitiesCsvModel> records);

    Task<Stream> WriteToCsv(IEnumerable<Gen2EpicsCsvModel> records);

    Task<Guid> AddToCache(Gen2CsvImportModel<Gen2CapabilitiesCsvModel> records);

    Task<Guid> AddToCache(Gen2CsvImportModel<Gen2EpicsCsvModel> records);

    Task<Gen2CsvImportModel<Gen2CapabilitiesCsvModel>> GetCachedCapabilities(Guid id);

    Task<Gen2CsvImportModel<Gen2EpicsCsvModel>> GetCachedEpics(Guid id);
}
