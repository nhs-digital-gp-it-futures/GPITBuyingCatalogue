using System;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class EnumExtensions
    {
        public static TAttribute GetAttributeFromEnumProperty<TAttribute>(this Enum thisEnum)
            where TAttribute : Attribute
        {
            var attributes = thisEnum.GetType().GetMember(thisEnum.ToString()).FirstOrDefault().GetCustomAttributes(typeof(TAttribute), false);

            if (attributes is null)
                throw new InvalidOperationException();

            return (TAttribute)attributes[0];
        }
    }
}
