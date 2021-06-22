using System;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public class CataloguePriceType
        : EnumerationBase
    {
        public static readonly CataloguePriceType Flat = new(1, "Flat");
        public static readonly CataloguePriceType Tiered = new(2, "Tiered");

        public CataloguePriceType(int id, string name)
            : base(id, name)
        {
        }
    }
}
