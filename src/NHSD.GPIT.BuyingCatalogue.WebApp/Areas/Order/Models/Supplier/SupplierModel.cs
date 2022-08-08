using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public sealed class SupplierModel : OrderingBaseModel
    {
        public SupplierModel(string internalOrgId, CallOffId callOffId, EntityFramework.Ordering.Models.Order order)
        {
            Title = "Supplier contact details";
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            SupplierId = order.Supplier.Id;
            SupplierName = order.Supplier.Name;
        }

        public SupplierModel()
        {
        }

        public CallOffId CallOffId { get; set; }

        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        public int? SelectedContactId { get; set; }

        public List<SupplierContact> Contacts { get; set; }

        public List<SelectableRadioOption<int?>> FormattedContacts => Contacts?
            .Select(x => new SelectableRadioOption<int?>(x.Description, x.Id))
            .ToList();

        public SupplierContact TemporaryContact => Contacts?.FirstOrDefault(x => x?.Id == SupplierContact.TemporaryContactId);
    }
}
