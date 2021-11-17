using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation
{
    public interface IUrlValidator
    {
        Task<bool> IsValidUrl(string url);
    }
}
