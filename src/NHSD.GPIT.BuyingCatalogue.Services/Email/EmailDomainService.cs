using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
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
}
