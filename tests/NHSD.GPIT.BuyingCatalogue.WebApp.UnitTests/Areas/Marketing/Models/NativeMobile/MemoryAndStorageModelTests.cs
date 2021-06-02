using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeMobile
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class MemoryAndStorageModelTests
    {
        private IMapper mapper;

        [OneTimeSetUp]
        public void SetUp()
        {
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            mapper = null;
        }

        [Test]
        public void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new MemoryAndStorageModel(null));
        }

        [Test]
        public void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication
            {
                MobileMemoryAndStorage = new MobileMemoryAndStorage
                {
                    MinimumMemoryRequirement = "1GB",
                    Description = "Storage requirements"
                }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = mapper.Map<CatalogueItem, MemoryAndStorageModel>(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123/section/native-mobile", model.BackLink);
            Assert.AreEqual("1GB", model.SelectedMemorySize);
            Assert.AreEqual("Storage requirements", model.Description);
            model.MemorySizes.Should().BeEquivalentTo(GetMemorySizes());
        }

        [Test]
        public void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new MemoryAndStorageModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.SelectedMemorySize);
        }

        [Test]
        [TestCase(null, null, false)]
        [TestCase("", "", false)]
        [TestCase(" ", " ", false)]
        [TestCase("Memory", null, false)]
        [TestCase(null, "Description", false)]
        [TestCase("Memory", "Description", true)]
        public void IsCompleteIsCorrectlySet(string memorySize, string description, bool? expected)
        {
            var clientApplication = new ClientApplication
            {
                MobileMemoryAndStorage = new MobileMemoryAndStorage
                {
                    MinimumMemoryRequirement = memorySize,
                    Description = description
                }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = mapper.Map<CatalogueItem, MemoryAndStorageModel>(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }

        private static List<SelectListItem> GetMemorySizes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem{ Text = "256MB", Value = "256MB"},
                new SelectListItem{ Text = "512MB", Value = "512MB"},
                new SelectListItem{ Text = "1GB", Value = "1GB"},
                new SelectListItem{ Text = "2GB", Value = "2GB"},
                new SelectListItem{ Text = "4GB", Value = "4GB"},
                new SelectListItem{ Text = "8GB", Value = "8GB"},
                new SelectListItem{ Text = "16GB or higher", Value = "16GB or higher"}
            };
        }

        private static List<SelectListItem> GetScreenResolutions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem{ Text = "16:9 - 640 x 360", Value = "16:9 - 640 x 360" },
                new SelectListItem{ Text = "4:3 - 800 x 600", Value = "4:3 - 800 x 600" },
                new SelectListItem{ Text = "4:3 - 1024 x 768", Value = "4:3 - 1024 x 768" },
                new SelectListItem{ Text = "16:9 - 1280 x 720", Value = "16:9 - 1280 x 720" },
                new SelectListItem{ Text = "16:10 - 1280 x 800", Value = "16:10 - 1280 x 800" },
                new SelectListItem{ Text = "5:4 - 1280 x 1024", Value = "5:4 - 1280 x 1024" },
                new SelectListItem{ Text = "16:9 - 1360 x 768", Value = "16:9 - 1360 x 768" },
                new SelectListItem{ Text = "16:9 - 1366 x 768", Value = "16:9 - 1366 x 768" },
                new SelectListItem{ Text = "16:10 - 1440 x 900", Value = "16:10 - 1440 x 900" },
                new SelectListItem{ Text = "16:9 - 1536 x 864", Value = "16:9 - 1536 x 864" },
                new SelectListItem{ Text = "16:9 - 1600 x 900", Value = "16:9 - 1600 x 900" },
                new SelectListItem{ Text = "16:10 - 1680 x 1050", Value = "16:10 - 1680 x 1050" },
                new SelectListItem{ Text = "16:9 - 1920 x 1080", Value = "16:9 - 1920 x 1080" },
                new SelectListItem{ Text = "16:10 - 1920 x 1200", Value = "16:10 - 1920 x 1200" },
                new SelectListItem{ Text = "16:9 - 2048 x 1152", Value = "16:9 - 2048 x 1152" },
                new SelectListItem{ Text = "21:9 - 2560 x 1080", Value = "21:9 - 2560 x 1080" },
                new SelectListItem{ Text = "16:9 - 2560 x 1440", Value = "16:9 - 2560 x 1440" },
                new SelectListItem{ Text = "21:9 - 3440 x 1440", Value = "21:9 - 3440 x 1440" },
                new SelectListItem{ Text = "16:9 - 3840 x 2160", Value = "16:9 - 3840 x 2160" }
            };
        }
    }
}
