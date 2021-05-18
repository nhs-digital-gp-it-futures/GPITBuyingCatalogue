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

        public static void ValidateNotNullOrWhiteSpace(this string item, string name)
        {
            if (string.IsNullOrWhiteSpace(item))
                throw new ArgumentException("Argument IsNullOrWhiteSpace", name);
        }

        public static void ValidateGuid(this Guid item, string name)
        {
            if (item == Guid.Empty)
                throw new ArgumentException("Agrgument is empty Guid", name);
        }
    }
}
