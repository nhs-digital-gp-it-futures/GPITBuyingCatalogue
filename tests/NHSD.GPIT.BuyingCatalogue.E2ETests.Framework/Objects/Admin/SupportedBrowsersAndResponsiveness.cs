using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class SupportedBrowsersAndResponsiveness
    {
        public static By SelectSupportedBrowsersError => ByExtensions.DataTestId("supportedBrowsersError");
        public static By MobileResponsiveError => ByExtensions.DataTestId("mobileResponsive");

    }
}
