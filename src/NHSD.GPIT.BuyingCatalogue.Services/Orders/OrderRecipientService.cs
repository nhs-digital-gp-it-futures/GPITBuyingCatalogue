using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders;

public class OrderRecipientService : IOrderRecipientService
{
    private readonly IOrderService orderService;
    private readonly BuyingCatalogueDbContext dbContext;

    public OrderRecipientService(
        IOrderService orderService,
        BuyingCatalogueDbContext dbContext)
    {
        this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task SetOrderRecipients(string internalOrgId, CallOffId callOffId, IEnumerable<string> odsCodes)
    {
        var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

        var orderRecipients = wrapper.Order.OrderRecipients;

        var odsCodesExistingAndSelected = new List<string>();
        odsCodesExistingAndSelected.AddRange(odsCodes);
        odsCodesExistingAndSelected.AddRange(wrapper.ExistingOrderRecipients.Select(r => r.OdsCode));

        var staleRecipients = orderRecipients.Where(x => !odsCodesExistingAndSelected.Contains(x.OdsCode)).ToList();
        var newRecipients = odsCodesExistingAndSelected.Where(x => orderRecipients.All(y => x != y.OdsCode)).ToList();

        dbContext.OrderRecipients.RemoveRange(staleRecipients);
        dbContext.OrderRecipients.AddRange(newRecipients.Select(x => wrapper.InitialiseOrderRecipient(x)));

        await dbContext.SaveChangesAsync();
    }
}
