using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using OpenQA.Selenium;
using CommonSelectors = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.CommonSelectors;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Marketing
{
    public class PreviewActions : ActionBase
    {
        public PreviewActions(IWebDriver driver)
            : base(driver)
        {
        }

        public bool MainSectionDisplayed(string section)
        {
            return Driver.FindElements(CommonSelectors.Header2).Select(s => s.Text.ToLower())
                .Contains(section.ToLower());
        }

        public void ExpandSection(string applicationType)
        {
            Driver.FindElements(PreviewPageObjects.ExpandingSections)
                .Select(s => s.FindElement(By.TagName("summary")))
                .First(s => s.Text.Contains(applicationType))
                .Click();
        }

        public bool ExpandedSectionDisplayed(string applicationType, string section)
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
