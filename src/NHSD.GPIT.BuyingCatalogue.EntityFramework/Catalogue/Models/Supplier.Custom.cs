namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed partial class Supplier
    {
        public bool CanDeleteFromSupplierContacts() => !(IsActive && SupplierContacts.Count == 1);
    }
}
