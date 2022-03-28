using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class TermsOfUseObjects
    {
        internal static By SectionOne => ByExtensions.DataTestId("terms-section-one");

        internal static By SectionTwo => ByExtensions.DataTestId("terms-section-two");

        internal static By SectionThree => ByExtensions.DataTestId("terms-section-three");

        internal static By SectionFour => ByExtensions.DataTestId("terms-section-four");

        internal static By SectionFive => ByExtensions.DataTestId("terms-section-five");

        internal static By SectionSix => ByExtensions.DataTestId("terms-section-six");

        internal static By SectionSeven => ByExtensions.DataTestId("terms-section-seven");

        internal static By SectionEight => ByExtensions.DataTestId("terms-section-eight");

        internal static By SectionNine => ByExtensions.DataTestId("terms-section-nine");

        internal static By Form => ByExtensions.DataTestId("terms-form");

        internal static By HasAcceptedTermsOfUse => By.Id("HasAcceptedTermsOfUse");

        internal static By HasAcceptedPrivacyPolicy => By.Id("HasAcceptedPrivacyPolicy");

        internal static By HasOptedInUserResearch => By.Id("HasOptedInUserResearch");

        internal static By PrivacyPolicyCheckboxLink => By.LinkText("privacy policy (opens in a new tab)");

        internal static By PrivacyAndCookiesPolicyLink => By.LinkText("privacy policy and cookies policy");

        internal static By HomepageLink => ByExtensions.DataTestId("homepage-link");

        internal static By HomeCrumb => ByExtensions.DataTestId("homepage-link");
    }
}
