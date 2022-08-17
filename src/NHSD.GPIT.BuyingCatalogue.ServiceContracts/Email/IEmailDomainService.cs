using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

public interface IEmailDomainService
{
    Task<IEnumerable<EmailDomain>> GetAllowedDomains();

    Task<EmailDomain> GetAllowedDomain(int id);

    Task AddAllowedDomain(string domain);

    Task DeleteAllowedDomain(int id);

    Task<bool> Exists(string domain);

    Task<bool> IsAllowed(string emailAddress);
}
