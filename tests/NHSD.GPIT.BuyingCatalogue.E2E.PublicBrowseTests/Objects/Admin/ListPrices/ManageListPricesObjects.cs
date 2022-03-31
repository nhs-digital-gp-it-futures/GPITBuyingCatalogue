using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ListPrices
{
    internal static class ManageListPricesObjects
    {
        internal static By AddPriceLink => By.LinkText("Add a list price");

        internal static By TieredPrices => ByExtensions.DataTestId("tiered-prices");

        internal static By FlatPrices => ByExtensions.DataTestId("flat-prices");

        internal static By TieredPrice(int cataloguePriceId) => ByExtensions.DataTestId($"tiered-price-{cataloguePriceId}");

        internal static By FlatPrice(int cataloguePriceId) => ByExtensions.DataTestId($"flat-price-{cataloguePriceId}");

        internal static By EditPriceLink(int cataloguePriceId) => ByExtensions.DataTestId($"edit-price-{cataloguePriceId}");
    }
}
