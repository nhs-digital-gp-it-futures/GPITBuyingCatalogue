using System.IO;

namespace NHSD.GPIT.BuyingCatalogue.Services.Document
{
    public interface IDocument
    {
        Stream Content { get; }

        string ContentType { get; }
    }
}
