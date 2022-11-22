namespace OrganisationImporter.Interfaces;

public interface IZipService
{
    Task<Stream> GetTrudDataFileAsync(Stream zipStream);
}
