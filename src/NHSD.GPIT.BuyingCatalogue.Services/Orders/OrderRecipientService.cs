using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders;

public class OrderRecipientService : IOrderRecipientService
{
    private readonly BuyingCatalogueDbContext dbContext;

    public OrderRecipientService(
        BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task SetOrderRecipients(string internalOrgId, CallOffId callOffId, IEnumerable<string> odsCodes)
    {
        var order = await dbContext.Orders
            .Include(x => x.OrderRecipients)
            .FirstOrDefaultAsync(
            x => x.OrderNumber == callOffId.OrderNumber
                && x.Revision == callOffId.Revision
                && x.OrderingParty.InternalIdentifier == internalOrgId);

        var orderRecipients = order.OrderRecipients;

        var staleRecipients = orderRecipients.Where(x => !odsCodes.Contains(x.OdsCode)).ToList();
        var newRecipients = odsCodes.Where(x => orderRecipients.All(y => x != y.OdsCode)).ToList();

        dbContext.OrderRecipients.RemoveRange(staleRecipients);
        dbContext.OrderRecipients.AddRange(newRecipients.Select(x => new OrderRecipient(order.Id, x)));

        await dbContext.SaveChangesAsync();
    }
}
