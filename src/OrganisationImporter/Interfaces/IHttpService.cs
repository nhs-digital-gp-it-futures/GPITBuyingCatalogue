namespace OrganisationImporter.Interfaces;

public interface IHttpService
{
    Task<Stream> DownloadAsync(Uri url);
}
