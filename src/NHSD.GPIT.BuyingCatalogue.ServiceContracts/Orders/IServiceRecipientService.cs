using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IServiceRecipientService
    {
        Task AddServiceRecipient(ServiceRecipientDto recipient);
    }
}
