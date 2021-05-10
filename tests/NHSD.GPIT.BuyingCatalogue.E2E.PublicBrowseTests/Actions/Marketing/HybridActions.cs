using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using OpenQA.Selenium;
using System;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal sealed class HybridActions : ActionBase
    {
        public HybridActions(IWebDriver driver) : base(driver)
        {
        }

        internal string EnterSummary(int charCount)
        {
            var summary = Strings.RandomString(charCount);

            Driver.FindElement(HostingTypesObjects.HybridHostingType_Summary).SendKeys(summary);

            return summary;
        }

        internal string EnterLink(int charCount)
        {
            var link = Strings.RandomString(charCount);

            Driver.FindElement(HostingTypesObjects.HybridHostingType_Link).SendKeys(link);

            return link;
        }

        internal string EnterHostingModel(int charCount)
        {
            var hostingModel = Strings.RandomString(charCount);

            Driver.FindElement(HostingTypesObjects.HybridHostingType_HostingModel).SendKeys(hostingModel);

            return hostingModel;
        }
    }
}
