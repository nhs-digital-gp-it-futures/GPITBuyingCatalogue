using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Preview
{
    public sealed class AppearOnPreview : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AppearOnPreview(LocalWebApplicationFactory factory)
            : base(factory, "solutions/preview/99999-001")
        {
        }

        [Theory]
        [InlineData("Description")]
        [InlineData("Features")]
        [InlineData("Client application type")]
        [InlineData("Roadmap")]
        [InlineData("Hosting type")]
        [InlineData("Contact details")]
        [InlineData("About supplier")]
        [InlineData("Integrations")]
        [InlineData("Implementation")]
        public void AppearOnPreview_MainSections(string section)
        {
            MarketingPages.PreviewActions.MainSectionDisplayed(section).Should().BeTrue();
        }

        [Theory]
        [InlineData("Browser-based", "Supported browser types")]
        [InlineData("Browser-based", "Mobile responsive")]
        [InlineData("Browser-based", "Mobile first approach")]
        [InlineData("Browser-based", "Plug-ins or extensions required")]
        [InlineData("Browser-based", "Additional information about plug-ins or extensions")]
        [InlineData("Browser-based", "Minimum connection speed")]
        [InlineData("Browser-based", "Screen resolution and aspect ratio")]
        [InlineData("Browser-based", "Hardware requirements")]
        [InlineData("Browser-based", "Additional information")]
        [InlineData("Native mobile or tablet", "Supported operating systems")]
        [InlineData("Native mobile or tablet", "Description of supported operating systems")]
        [InlineData("Native mobile or tablet", "Minimum connection speed")]
        [InlineData("Native mobile or tablet", "Connection types supported")]
        [InlineData("Native mobile or tablet", "Connection requirements")]
        [InlineData("Native mobile or tablet", "Memory size")]
        [InlineData("Native mobile or tablet", "Storage space")]
        [InlineData("Native mobile or tablet", "Third-party components")]
        [InlineData("Native mobile or tablet", "Device capabilities")]
        [InlineData("Native mobile or tablet", "Hardware requirements")]
        [InlineData("Native mobile or tablet", "Additional information")]
        [InlineData("Native desktop", "Description of supported operating systems")]
        [InlineData("Native desktop", "Minimum connection speed")]
        [InlineData("Native desktop", "Memory size")]
        [InlineData("Native desktop", "Storage space")]
        [InlineData("Native desktop", "Processing power")]
        [InlineData("Native desktop", "Screen resolution and aspect ratio")]
        [InlineData("Native desktop", "Third-party components")]
        [InlineData("Native desktop", "Device capabilities")]
        [InlineData("Native desktop", "Hardware requirements")]
        [InlineData("Native desktop", "Additional information")]
        public void AppearOnPreview_ClientApplicationTypes(string applicationType, string section)
        {
            MarketingPages.PreviewActions.ExpandSection(applicationType);
            MarketingPages.PreviewActions.ExpandedSectionDisplayed(applicationType, section).Should().BeTrue();
        }

        [Theory]
        [InlineData("Hybrid")]
        [InlineData("On premise")]
        [InlineData("Private cloud")]
        [InlineData("Public cloud")]
        public void AppearOnPreview_HostingTypes(string hostingType)
        {
            MarketingPages.PreviewActions.ExpandSection(hostingType);
            MarketingPages.PreviewActions.ExpandedSectionDisplayed(hostingType, "Summary").Should().BeTrue();
        }
    }
}
