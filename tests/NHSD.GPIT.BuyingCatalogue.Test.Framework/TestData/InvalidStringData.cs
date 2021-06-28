using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData
{
    public static class InvalidStringData
    {
        public static IEnumerable<object[]> TestData()
        {
            yield return new object[1];
            yield return new object[] { string.Empty };
            yield return new object[] { "\t" };
        }
    }
}
