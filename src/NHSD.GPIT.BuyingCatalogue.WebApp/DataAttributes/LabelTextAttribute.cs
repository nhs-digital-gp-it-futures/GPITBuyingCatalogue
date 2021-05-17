using System;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class LabelTextAttribute : Attribute
    {
        public LabelTextAttribute(string labelText)
        {
            Text = labelText;
        }

        public string Text { get; init; }
    }
}
