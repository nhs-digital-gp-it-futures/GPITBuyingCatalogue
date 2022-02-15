namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf
{
    public interface IPdfService
    {
        byte[] Convert(System.Uri url);
    }
}
