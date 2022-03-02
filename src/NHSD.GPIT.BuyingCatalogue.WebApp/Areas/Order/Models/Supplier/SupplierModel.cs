using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

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

        public List<SelectListItem> FormattedContacts => Contacts?
            .Select(x => new SelectListItem(FormatSupplierContact(x), $"{x?.Id}"))
            .ToList();

        public SupplierContact TemporaryContact => Contacts?.FirstOrDefault(x => x?.Id == SupplierContact.TemporaryContactId);

        private static string FormatSupplierContact(SupplierContact contact)
        {
            if (contact == null)
                return string.Empty;

            var output = $"{contact.FirstName} {contact.LastName}";

            return string.IsNullOrWhiteSpace(contact.Department)
                ? output
                : $"{output} ({contact.Department})";
        }
    }
}
