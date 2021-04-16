using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Objects
{
    internal sealed class CustomBy : By
    {
        internal static By DataTestId(string dataTestId, string childCssSelector = null)
        {
            var selectorString = $"[data-test-id={dataTestId}] {childCssSelector}".Trim();

            return CssSelector(selectorString);
        }
    }
}
