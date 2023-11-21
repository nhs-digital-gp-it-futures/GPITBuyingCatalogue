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

        var currentOrderRecipients = wrapper.Order.OrderRecipients;

        var odsCodesExistingAndSelected = new List<string>(odsCodes);

        // for newly created amendments we dont need to add in the
        // previous recipients as we add all the recipients when the
        // amended order is created however there might be some in
        // progress amendments that this would help with
        odsCodesExistingAndSelected.AddRange(wrapper.PreviousRecipientsOdsCodes());

        var staleRecipients = currentOrderRecipients.Where(x => !odsCodesExistingAndSelected.Contains(x.OdsCode)).ToList();
        var newRecipients = odsCodesExistingAndSelected.Where(x => currentOrderRecipients.All(y => x != y.OdsCode)).ToList();

        dbContext.OrderRecipients.RemoveRange(staleRecipients);
        dbContext.OrderRecipients.AddRange(newRecipients.Select(x => wrapper.InitialiseOrderRecipient(x)));

        await dbContext.SaveChangesAsync();
    }
}
