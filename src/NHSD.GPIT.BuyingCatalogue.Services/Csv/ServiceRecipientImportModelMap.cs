using CsvHelper.Configuration;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv;

public sealed class ServiceRecipientImportModelMap : ClassMap<ServiceRecipientImportModel>
{
    public ServiceRecipientImportModelMap()
    {
        Map(o => o.Organisation).Index(0).Name("Organisation Name");
        Map(o => o.OdsCode).Index(1).Name("ODS Code");
    }
}
