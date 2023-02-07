namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;

public readonly struct BlobDocument
{
    public BlobDocument(
        string containerName,
        string document)
    {
        ContainerName = containerName;
        Document = document;
    }

    public string ContainerName { get; }

    public string Document { get; }
}
