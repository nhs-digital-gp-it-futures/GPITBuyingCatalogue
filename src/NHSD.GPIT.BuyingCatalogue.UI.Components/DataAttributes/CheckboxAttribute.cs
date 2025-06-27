using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CheckboxAttribute : Attribute
    {
        public CheckboxAttribute(string displayText, [CallerMemberName] string propertyName = null)
        {
            DisplayText = displayText;

            FieldText = PascalCaseToKebabCase(propertyName);
        }

        public string DisplayText { get; init; }

        public string FieldText { get; init; }

        private static string PascalCaseToKebabCase(string input)
        {
            return string.IsNullOrEmpty(input)
                ? string.Empty
                : string.Join("-", RegularExpressions.KebabNameRegex().Matches(input)).ToLower();
        }
    }
}
