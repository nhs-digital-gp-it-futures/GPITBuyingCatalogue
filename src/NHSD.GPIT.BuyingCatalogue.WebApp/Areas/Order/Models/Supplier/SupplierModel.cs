using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public sealed class SupplierModel : OrderingBaseModel
    {
        public SupplierModel(string odsCode, EntityFramework.Ordering.Models.Order order, ICollection<SupplierContact> supplierContacts)
        {
            Title = $"Supplier information for {order.CallOffId}";
            OdsCode = odsCode;
            Id = order.Supplier.Id;
            Name = order.Supplier.Name;
            Address = order.Supplier.Address;

            if (order.SupplierContact == null && supplierContacts.Any())
            {
                PrimaryContact = new Contact
                {
                    FirstName = supplierContacts.First().FirstName,
                    LastName = supplierContacts.First().LastName,
                    Email = supplierContacts.First().Email,
                    Phone = supplierContacts.First().PhoneNumber,
                };
            }
            else
            {
                PrimaryContact = order.SupplierContact;
            }
        }

        public SupplierModel()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }

        public Contact PrimaryContact { get; init; }
    }
}
