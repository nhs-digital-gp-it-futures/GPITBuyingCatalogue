using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;

[ExcludeFromCodeCoverage]
public class AzureBlobSettings
{
    public string ConnectionString { get; set; }

    public string OrderPdfContainerName { get; set; }
}
