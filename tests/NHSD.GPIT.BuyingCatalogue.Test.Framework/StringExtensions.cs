using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework
{
    public static class StringExtensions
    {
        public static string ToControllerName(this string input) => input.Replace("Controller", null);

        public static string ToLowerCaseHyphenated(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException(nameof(input));

            var chars = new List<char> { input[0] };
            foreach (var item in input.Skip(1))
            {
                if (char.IsUpper(item))
                    chars.Add(('-'));
                chars.Add(item);
            }

            return string.Join("", chars).ToLower();
        }
    }
}
