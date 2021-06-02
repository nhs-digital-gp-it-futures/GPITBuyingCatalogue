namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public class CatalogueItemType
        : EnumerationBase
    {
        public static readonly CatalogueItemType Solution = new(1, "Solution");
        public static readonly CatalogueItemType AdditionalService = new(2, "Additional Service");
        public static readonly CatalogueItemType AssociatedService = new(3, "Associated Service");

        public CatalogueItemType(int id, string name)
            : base(id, name)
        {
        }
    }
}
