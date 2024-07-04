using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

public interface IEmailPreferenceService
{
    Task<List<UserEmailPreferenceModel>> Get(int userId);

    Task Save(int userId, ICollection<UserEmailPreferenceModel> preferences);
}
