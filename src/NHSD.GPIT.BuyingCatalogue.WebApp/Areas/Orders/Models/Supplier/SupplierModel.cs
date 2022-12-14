using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier
{
    public sealed class SupplierModel : OrderingBaseModel
    {
        public SupplierModel(string internalOrgId, CallOffId callOffId, Order order)
        {
            Title = "Supplier contact details";
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            IsAmendment = order.IsAmendment;
            SupplierId = order.Supplier.Id;
            SupplierName = order.Supplier.Name;
        }

        public SupplierModel()
        {
        }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment { get; set; }

        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        public int? SelectedContactId { get; set; }

        public List<SupplierContact> Contacts { get; set; }

        public List<SelectOption<int?>> FormattedContacts => Contacts?
            .Select(x => new SelectOption<int?>(x.Description, x.Id))
            .ToList();

        public SupplierContact TemporaryContact => Contacts?.FirstOrDefault(x => x?.Id == SupplierContact.TemporaryContactId);
    }
}
