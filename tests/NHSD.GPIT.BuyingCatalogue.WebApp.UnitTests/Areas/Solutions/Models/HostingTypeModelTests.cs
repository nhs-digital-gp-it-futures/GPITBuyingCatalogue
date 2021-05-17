using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HostingTypeModelTests
    {
        [Test]
        public static void WithValidPublicCloud_PropertiesAreSet()
        {
            var publicCloud = new PublicCloud
            {
                Summary = "A summary",
                Link = "A link",
                RequiresHscn = "Requires Hscn"
            };

            var model = new HostingTypeModel(publicCloud);

            Assert.AreEqual("Public cloud", model.Label);
            Assert.AreEqual("public-cloud", model.DataTestTag);
            Assert.AreEqual("A summary", model.Summary);
            Assert.AreEqual("A link", model.Link);
            Assert.AreEqual("Requires Hscn", model.RequiresHscn);
        }

        [Test]
        public static void WithValidPrivateCloud_PropertiesAreSet()
        {
            var privateCloud = new PrivateCloud
            {
                Summary = "A summary",
                Link = "A link",
                RequiresHscn = "Requires Hscn",
                HostingModel = "Hosting Model"
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.AreEqual("Private cloud", model.Label);
            Assert.AreEqual("private-cloud", model.DataTestTag);
            Assert.AreEqual("A summary", model.Summary);
            Assert.AreEqual("A link", model.Link);
            Assert.AreEqual("Requires Hscn", model.RequiresHscn);
            Assert.AreEqual("Hosting Model", model.HostingModel);
        }

        [Test]
        public static void WithValidHybridCloud_PropertiesAreSet()
        {
            var hybridHostingType = new HybridHostingType
            {
                Summary = "A summary",
                Link = "A link",
                RequiresHscn = "Requires Hscn",
                HostingModel = "Hosting Model"
            };

            var model = new HostingTypeModel(hybridHostingType);

            Assert.AreEqual("Hybrid", model.Label);
            Assert.AreEqual("hybrid", model.DataTestTag);
            Assert.AreEqual("A summary", model.Summary);
            Assert.AreEqual("A link", model.Link);
            Assert.AreEqual("Requires Hscn", model.RequiresHscn);
            Assert.AreEqual("Hosting Model", model.HostingModel);
        }

        [Test]
        [TestCase(null, null, null, null, false)]
        [TestCase("", "", "", "", false)]
        [TestCase(" ", " ", " ", " ", false)]
        [TestCase("A summary", null, null, null, true)]
        [TestCase(null, "A link", null, null, true)]
        [TestCase(null, null, "Requires Hscn", null, true)]
        [TestCase(null, null, null, "A hosting model", true)]
        public static void DisplayHostingType_CorrectlySet(string summary, string link, string requiresHscn, string hostingModel, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                Summary = summary,
                Link = link,
                RequiresHscn = requiresHscn,
                HostingModel = hostingModel
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.AreEqual(expected, model.DisplayHostingType);
        }

        [Test]
        [TestCase(null, null, false)]
        [TestCase("", "", false)]
        [TestCase(" ", " ", false)]
        [TestCase("A summary", null, true)]
        [TestCase(null, "A link", true)]
        public static void DisplaySummary_CorrectlySet(string summary, string link, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                Summary = summary,
                Link = link
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.AreEqual(expected, model.DisplaySummary);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("A summary", true)]
        public static void DisplaySummaryDescription_CorrectlySet(string summary, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                Summary = summary
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.AreEqual(expected, model.DisplaySummaryDescription);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("A link", true)]
        public static void DisplayLink_CorrectlySet(string link, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                Link = link
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.AreEqual(expected, model.DisplayLink);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("A hosting model", true)]
        public static void DisplayHostingModel_CorrectlySet(string hostingModel, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                HostingModel = hostingModel
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.AreEqual(expected, model.DisplayHostingModel);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("Requires Hscn", true)]
        public static void DisplayRequiresHscn_CorrectlySet(string requiresHscn, bool expected)
        {
            var privateCloud = new PrivateCloud
            {
                RequiresHscn = requiresHscn
            };

            var model = new HostingTypeModel(privateCloud);

            Assert.AreEqual(expected, model.DisplayRequiresHscn);
        }
    }
}
