using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PasswordAttribute : DataTypeAttribute
    {
        public PasswordAttribute()
            : base(DataType.Password)
        {
        }
    }
}
