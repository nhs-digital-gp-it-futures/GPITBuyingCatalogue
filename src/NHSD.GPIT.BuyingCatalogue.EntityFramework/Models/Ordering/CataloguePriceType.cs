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

        public static CataloguePriceType Parse(string name)
        {
            if (name.Equals(nameof(Flat), System.StringComparison.InvariantCultureIgnoreCase))
                return Flat;
            else if (name.Equals(nameof(Tiered), System.StringComparison.InvariantCultureIgnoreCase))
                return Tiered;

            throw new ArgumentException("Invalid CataloguePriceType", nameof(name));
        }
    }
}
