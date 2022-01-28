using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class NewContactModel : OrderingBaseModel
    {
        public NewContactModel()
        {
        }

        public NewContactModel(CallOffId callOffId, int supplierId, string supplierName)
        {
            CallOffId = callOffId;
            SupplierId = supplierId;
            SupplierName = supplierName;
        }

        public CallOffId CallOffId { get; set; }

        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        [StringLength(35)]
        public string FirstName { get; set; }

        [StringLength(35)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string Department { get; set; }

        [StringLength(35)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Email { get; set; }
    }
}
