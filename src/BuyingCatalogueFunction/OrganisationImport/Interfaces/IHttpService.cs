using System;
using System.IO;
using System.Threading.Tasks;

namespace BuyingCatalogueFunction.OrganisationImport.Interfaces;

public interface IHttpService
{
    Task<Stream> DownloadAsync(Uri url);
}
