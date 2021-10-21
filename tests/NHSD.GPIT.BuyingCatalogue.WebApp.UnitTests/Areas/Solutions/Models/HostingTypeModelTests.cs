using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class HostingTypeModelTests
    {
        [Fact]
        public static void WithValidPublicCloud_PropertiesAreSet()
        {
            var publicCloud = new PublicCloud
            {
                Summary = "A summary",
                Link = "A link",
                RequiresHscn = "Requires Hscn",
            };

            var model = new HostingTypeModel(publicCloud);

            Assert.Equal("Public cloud", model.Label);
            Assert.Equal("public-cloud", model.DataTestTag);
            Assert.Equal("A summary", model.Summary);
            Assert.Equal("A link", model.Link);
            Assert.Equal("Requires Hscn", model.RequiresHscn);
        }

        [Fact]
        public static void WithValidPrivateCloud_PropertiesAreSet()
        {
            var privateCloud = new PrivateCloud
            {
                Summary = "A summary",
                Link = "A link",
                RequiresHscn = "Requires Hscn",
                HostingModel = "Hosting Model",
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.Equal("Private cloud", model.Label);
            Assert.Equal("private-cloud", model.DataTestTag);
            Assert.Equal("A summary", model.Summary);
            Assert.Equal("A link", model.Link);
            Assert.Equal("Requires Hscn", model.RequiresHscn);
            Assert.Equal("Hosting Model", model.HostingModel);
        }

        [Fact]
        public static void WithValidHybridCloud_PropertiesAreSet()
        {
            var hybridHostingType = new HybridHostingType
            {
                Summary = "A summary",
                Link = "A link",
                RequiresHscn = "Requires Hscn",
                HostingModel = "Hosting Model",
            };

            var model = new HostingTypeModel(hybridHostingType);

            Assert.Equal("Hybrid", model.Label);
            Assert.Equal("hybrid", model.DataTestTag);
            Assert.Equal("A summary", model.Summary);
            Assert.Equal("A link", model.Link);
            Assert.Equal("Requires Hscn", model.RequiresHscn);
            Assert.Equal("Hosting Model", model.HostingModel);
        }

        [Theory]
        [InlineData(null, null, null, null, false)]
        [InlineData("", "", "", "", false)]
        [InlineData(" ", " ", " ", " ", false)]
        [InlineData("A summary", null, null, null, true)]
        [InlineData(null, "A link", null, null, true)]
        [InlineData(null, null, "Requires Hscn", null, true)]
        [InlineData(null, null, null, "A hosting model", true)]
        public static void DisplayHostingType_CorrectlySet(string summary, string link, string requiresHscn, string hostingModel, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                Summary = summary,
                Link = link,
                RequiresHscn = requiresHscn,
                HostingModel = hostingModel,
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.Equal(expected, model.DisplayHostingType);
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData("", "", false)]
        [InlineData(" ", " ", false)]
        [InlineData("A summary", null, true)]
        [InlineData(null, "A link", true)]
        public static void DisplaySummary_CorrectlySet(string summary, string link, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                Summary = summary,
                Link = link,
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.Equal(expected, model.DisplaySummary);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("A summary", true)]
        public static void DisplaySummaryDescription_CorrectlySet(string summary, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                Summary = summary,
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.Equal(expected, model.DisplaySummaryDescription);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("A link", true)]
        public static void DisplayLink_CorrectlySet(string link, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                Link = link,
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.Equal(expected, model.DisplayLink);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("A hosting model", true)]
        public static void DisplayHostingModel_CorrectlySet(string hostingModel, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                HostingModel = hostingModel,
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.Equal(expected, model.DisplayHostingModel);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("Requires Hscn", true)]
        public static void DisplayRequiresHscn_CorrectlySet(string requiresHscn, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                RequiresHscn = requiresHscn,
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.Equal(expected, model.DisplayRequiresHscn);
        }
    }
}
