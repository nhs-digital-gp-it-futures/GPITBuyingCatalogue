using System;
using System.ComponentModel;
using System.Globalization;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.TypeConverters
{
    public sealed class CatalogueItemIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
    sourceType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            CatalogueItemId.ParseExact((string)value);
    }
}
