using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
                return null;

            FieldInfo field = type.GetField(name);
            if (field == null)
                return null;

            return Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attr
                ? attr.Name
                : value.ToString();
        }
    }
}
