using System;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class TextAreaRowsAttribute : Attribute
    {
        public TextAreaRowsAttribute(int numberOfRows)
        {
            NumberOfRows = numberOfRows;
        }
        public int NumberOfRows { get; init; }
    }
}
