using System;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class LabelHintAttribute : Attribute
    {
        public LabelHintAttribute(string hintText)
        {
            Text = hintText;
        }

        public virtual string Text { get; init; }
    }
}
