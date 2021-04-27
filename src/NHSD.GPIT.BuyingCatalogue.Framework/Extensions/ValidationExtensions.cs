using System;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class ValidationExtensions
    {
        public static void ValidateNotNull(this object item, string name)
        {
            if (item == null)
                throw new ArgumentNullException(name);
        }
    }
}