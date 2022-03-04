using System;
using System.IO;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Streaming
{
    public interface IStreamingService
    {
        Task<Stream> StreamContents(Uri uri);
    }
}
