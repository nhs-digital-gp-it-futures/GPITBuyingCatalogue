using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes
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
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
            return string.Join("-", pattern.Matches(input)).ToLower();
        }
    }
}
