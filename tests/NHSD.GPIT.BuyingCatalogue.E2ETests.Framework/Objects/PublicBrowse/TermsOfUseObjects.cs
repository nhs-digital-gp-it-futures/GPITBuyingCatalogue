using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class TermsOfUseObjects
    {
        public static By SectionOne => ByExtensions.DataTestId("terms-section-one");

        public static By SectionTwo => ByExtensions.DataTestId("terms-section-two");

        public static By SectionThree => ByExtensions.DataTestId("terms-section-three");

        public static By SectionFour => ByExtensions.DataTestId("terms-section-four");

        public static By SectionFive => ByExtensions.DataTestId("terms-section-five");

        public static By SectionSix => ByExtensions.DataTestId("terms-section-six");

        public static By SectionSeven => ByExtensions.DataTestId("terms-section-seven");

        public static By SectionEight => ByExtensions.DataTestId("terms-section-eight");

        public static By SectionNine => ByExtensions.DataTestId("terms-section-nine");

        public static By Form => ByExtensions.DataTestId("terms-form");

        public static By HasAcceptedTermsOfUse => By.Id("HasAcceptedTermsOfUse");

        public static By HasAcceptedPrivacyPolicy => By.Id("HasAcceptedPrivacyPolicy");

        public static By HasOptedInUserResearch => By.Id("HasOptedInUserResearch");

        public static By PrivacyPolicyCheckboxLink => By.LinkText("privacy policy (opens in a new tab)");

        public static By PrivacyAndCookiesPolicyLink => By.LinkText("privacy policy and cookies policy");

        public static By HomepageLink => ByExtensions.DataTestId("homepage-link");

        public static By HomeCrumb => ByExtensions.DataTestId("homepage-link");
    }
}
