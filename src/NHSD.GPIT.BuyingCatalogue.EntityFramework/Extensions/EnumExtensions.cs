using System;
using EnumsNET;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class EnumExtensions
    {
        public static string Description<TEnum>(this TEnum value)
            where TEnum : struct, Enum => value.AsString(EnumFormat.Description);

        public static string Name<TEnum>(this TEnum value)
            where TEnum : struct, Enum => value.AsString(EnumFormat.DisplayName);

        public static string EnumMemberName<TEnum>(this TEnum value)
            where TEnum : struct, Enum => value.AsString(EnumFormat.EnumMemberValue);

        public static string GetEnumMemberValue<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            return value.EnumMemberName();
        }
    }
}
