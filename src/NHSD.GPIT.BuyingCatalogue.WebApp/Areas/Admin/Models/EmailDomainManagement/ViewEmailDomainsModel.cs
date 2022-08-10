using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.EmailDomainManagement;

public class ViewEmailDomainsModel
{
    public ViewEmailDomainsModel(IEnumerable<EmailDomain> emailDomains)
    {
        EmailDomains = emailDomains;
    }

    public IEnumerable<EmailDomain> EmailDomains { get; set; }
}
