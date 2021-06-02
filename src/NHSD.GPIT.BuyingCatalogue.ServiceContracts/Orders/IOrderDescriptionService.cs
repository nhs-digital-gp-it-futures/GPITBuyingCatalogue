using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderDescriptionService
    {
        Task SetOrderDescription(string callOffId, string description);
    }
}
