using System;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

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

        public static SupportedIntegrations ToIntegrationId(this int value) => value switch
        {
            0 => SupportedIntegrations.Im1,
            1 => SupportedIntegrations.GpConnect,
            2 => SupportedIntegrations.NhsApp,
            _ => throw new ArgumentOutOfRangeException(nameof(value)),
        };
    }
}
