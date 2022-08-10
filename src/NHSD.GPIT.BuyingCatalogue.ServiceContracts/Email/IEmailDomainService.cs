using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

public interface IEmailDomainService
{
    Task<IEnumerable<EmailDomain>> GetAllowedDomains();

    Task AddAllowedDomain(string domain);

    Task RemoveAllowedDomain(int id);
}
