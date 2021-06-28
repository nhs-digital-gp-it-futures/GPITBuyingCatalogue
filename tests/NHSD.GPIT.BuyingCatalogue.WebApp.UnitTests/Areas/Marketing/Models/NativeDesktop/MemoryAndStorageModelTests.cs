using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.NativeDesktop
{
    public sealed class MemoryAndStorageModelTests : IDisposable
    {
        private IMapper mapper;

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new MemoryAndStorageModel();

            Assert.Equal("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.SelectedMemorySize);
            Assert.Null(model.StorageDescription);
            Assert.Null(model.SelectedScreenResolution);
        }

        public MemoryAndStorageModelTests()
        {
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeDesktopProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
        }

        public void Dispose()
        {
            mapper = null;
        }

        [Fact]
        public void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new MemoryAndStorageModel(null));
        }

        [Fact]
        public void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopMemoryAndStorage = new NativeDesktopMemoryAndStorage
                {
                    MinimumMemoryRequirement = "1GB",
                    StorageRequirementsDescription = "Storage requirements",
                    MinimumCpu = "Xeon",
                    RecommendedResolution = "4:3 - 1024 x 768",
                }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { ClientApplication = json },
            };

            var model = mapper.Map<CatalogueItem, MemoryAndStorageModel>(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123/section/native-desktop", model.BackLink);
            Assert.Equal("1GB", model.SelectedMemorySize);
            Assert.Equal("Storage requirements", model.StorageDescription);
            Assert.Equal("Xeon", model.MinimumCpu);
            Assert.Equal("4:3 - 1024 x 768", model.SelectedScreenResolution);
            model.MemorySizes.Should().BeEquivalentTo(GetMemorySizes());
            model.ScreenResolutions.Should().BeEquivalentTo(GetScreenResolutions());
        }

        [Theory]
        [InlineData(null, null, null, null, false)]
        [InlineData("", "", "", "", false)]
        [InlineData(" ", " ", " ", " ", false)]
        [InlineData(null, null, null, "Resolution", false)]
        [InlineData("Memory", null, null, null, false)]
        [InlineData(null, "Description", null, null, false)]
        [InlineData(null, null, "Minimum Cpu", null, false)]
        [InlineData("Memory", "Description", null, null, false)]
        [InlineData("Memory", null, "Minimum Cpu", null, false)]
        [InlineData(null, "Description", "Minimum Cpu", null, false)]
        [InlineData("Memory", "Description", "Minimum Cpu", null, true)]
        [InlineData("Memory", "Description", "Minimum Cpu", "Resolution", true)]
        public void IsCompleteIsCorrectlySet(string memorySize, string description, string minimumCpu, string resolution, bool? expected)
        {
            var clientApplication = new ClientApplication
            {
                NativeDesktopMemoryAndStorage = new NativeDesktopMemoryAndStorage
                {
                    MinimumMemoryRequirement = memorySize,
                    StorageRequirementsDescription = description,
                    MinimumCpu = minimumCpu,
                    RecommendedResolution = resolution,
                }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = mapper.Map<CatalogueItem, MemoryAndStorageModel>(catalogueItem);

            Assert.Equal(expected, model.IsComplete);
        }

        private static IEnumerable<SelectListItem> GetMemorySizes()
        {
            return new List<SelectListItem>
            {
                new() { Text = "256MB", Value = "256MB"},
                new() { Text = "512MB", Value = "512MB"},
                new() { Text = "1GB", Value = "1GB"},
                new() { Text = "2GB", Value = "2GB"},
                new() { Text = "4GB", Value = "4GB"},
                new() { Text = "8GB", Value = "8GB"},
                new() { Text = "16GB or higher", Value = "16GB or higher"},
            };
        }

        private static IEnumerable<SelectListItem> GetScreenResolutions()
        {
            return new List<SelectListItem>
            {
                new() { Text = "16:9 - 640 x 360", Value = "16:9 - 640 x 360" },
                new() { Text = "4:3 - 800 x 600", Value = "4:3 - 800 x 600" },
                new() { Text = "4:3 - 1024 x 768", Value = "4:3 - 1024 x 768" },
                new() { Text = "16:9 - 1280 x 720", Value = "16:9 - 1280 x 720" },
                new() { Text = "16:10 - 1280 x 800", Value = "16:10 - 1280 x 800" },
                new() { Text = "5:4 - 1280 x 1024", Value = "5:4 - 1280 x 1024" },
                new() { Text = "16:9 - 1360 x 768", Value = "16:9 - 1360 x 768" },
                new() { Text = "16:9 - 1366 x 768", Value = "16:9 - 1366 x 768" },
                new() { Text = "16:10 - 1440 x 900", Value = "16:10 - 1440 x 900" },
                new() { Text = "16:9 - 1536 x 864", Value = "16:9 - 1536 x 864" },
                new() { Text = "16:9 - 1600 x 900", Value = "16:9 - 1600 x 900" },
                new() { Text = "16:10 - 1680 x 1050", Value = "16:10 - 1680 x 1050" },
                new() { Text = "16:9 - 1920 x 1080", Value = "16:9 - 1920 x 1080" },
                new() { Text = "16:10 - 1920 x 1200", Value = "16:10 - 1920 x 1200" },
                new() { Text = "16:9 - 2048 x 1152", Value = "16:9 - 2048 x 1152" },
                new() { Text = "21:9 - 2560 x 1080", Value = "21:9 - 2560 x 1080" },
                new() { Text = "16:9 - 2560 x 1440", Value = "16:9 - 2560 x 1440" },
                new() { Text = "21:9 - 3440 x 1440", Value = "21:9 - 3440 x 1440" },
                new() { Text = "16:9 - 3840 x 2160", Value = "16:9 - 3840 x 2160" },
            };
        }
    }
}
