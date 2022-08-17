using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Services.Email;

public class EmailDomainService : IEmailDomainService
{
    private readonly BuyingCatalogueDbContext dbContext;

    public EmailDomainService(BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<EmailDomain>> GetAllowedDomains()
        => await dbContext.EmailDomains.AsNoTracking().ToListAsync();

    public async Task<EmailDomain> GetAllowedDomain(int id)
        => await dbContext.EmailDomains.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);

    public async Task AddAllowedDomain(string domain)
    {
        dbContext.EmailDomains.Add(new EmailDomain(domain));

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAllowedDomain(int id)
    {
        var emailDomain = await GetAllowedDomain(id);
        if (emailDomain == null)
            return;

        dbContext.EmailDomains.Remove(emailDomain);

        await dbContext.SaveChangesAsync();
    }

    public Task<bool> Exists(string domain)
        => dbContext.EmailDomains.AsNoTracking().AnyAsync(d => d.Domain == domain);

    public async Task<bool> IsAllowed(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ArgumentNullException(nameof(emailAddress));

        var allowedDomains = (await GetAllowedDomains()).ToList();
        if (!allowedDomains.Any())
            return false;

        var emailDomain = emailAddress[emailAddress.IndexOf(EmailConstants.AddressCharacter, StringComparison.OrdinalIgnoreCase)..];

        var isValid = false;
        foreach (var allowedDomain in allowedDomains)
        {
            if (allowedDomain.Domain.Contains(EmailConstants.WildcardCharacter, StringComparison.OrdinalIgnoreCase))
            {
                var domainPart = allowedDomain.Domain[(allowedDomain.Domain.IndexOf(
                    EmailConstants.WildcardCharacter,
                    StringComparison.OrdinalIgnoreCase) + 1)..];

                isValid = emailDomain.EndsWith(domainPart, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                isValid = string.Equals(emailDomain, allowedDomain.Domain, StringComparison.OrdinalIgnoreCase);
            }

            if (isValid)
                break;
        }

        return isValid;
    }
}
