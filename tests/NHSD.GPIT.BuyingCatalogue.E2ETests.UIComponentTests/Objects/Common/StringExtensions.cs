using System;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects.Common
{
    public static class StringExtensions
    {
       
        public static string FormatForComparison(this string value) =>
            new(value.Where(c => !char.IsWhiteSpace(c)).ToArray());

       
    }
}
