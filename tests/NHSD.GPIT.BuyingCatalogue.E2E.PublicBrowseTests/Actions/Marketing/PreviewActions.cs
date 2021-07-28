using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal class PreviewActions : ActionBase
    {
        public PreviewActions(IWebDriver driver)
            : base(driver)
        {
        }

        internal bool MainSectionDisplayed(string section)
        {
            return Driver.FindElements(CommonSelectors.Header).Select(s => s.Text.ToLower())
                .Contains(section.ToLower());
        }

        internal void ExpandSection(string applicationType)
        {
            Driver.FindElements(PreviewPageObjects.ExpandingSections)
                .Select(s => s.FindElement(By.TagName("summary")))
                .Single(s => s.Text.Contains(applicationType))
                .Click();
        }

        internal bool ExpandedSectionDisplayed(string applicationType, string section)
        {
            return Driver.FindElements(GetPreviewSection(applicationType))
                .Select(s => s.Text).Contains(section, StringComparer.InvariantCultureIgnoreCase);
        }

        private static By GetPreviewSection(string section)
        {
            return section.ToLower() switch
            {
                "browser" or "browser based" or "browser-based" => PreviewPageObjects.BrowserBasedSectionTitles,
                "native mobile" or "mobile" or "native mobile or tablet" => PreviewPageObjects.NativeMobileSectionTitles,
                "native desktop" or "desktop" => PreviewPageObjects.NativeDesktopSectionTitles,
                _ => PreviewPageObjects.OtherSectionTitles,
            };
        }
    }
}
