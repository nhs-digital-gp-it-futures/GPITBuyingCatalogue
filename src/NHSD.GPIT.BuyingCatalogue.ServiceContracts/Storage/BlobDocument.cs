using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;

public readonly struct BlobDocument
{
    public BlobDocument(
        string containerName,
        string document)
    {
        ContainerName = containerName ?? throw new ArgumentNullException(nameof(containerName));
        DocumentName = document ?? throw new ArgumentNullException(nameof(document));
    }

    public string ContainerName { get; }

    public string DocumentName { get; }
}
