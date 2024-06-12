using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    public static class HostingTypesTests
    {
        public static IEnumerable<object[]> HostingTestData => new[]
        {
            new object[] { null, null, null, null, false },
            new object[] { null, new PrivateCloud(), new HybridHostingType(), new OnPremise(), false },
            new object[] { new PublicCloud(), null, new HybridHostingType(), new OnPremise(), false },
            new object[] { new PublicCloud(), new PrivateCloud(), null, new OnPremise(), false },
            new object[] { new PublicCloud(), new PrivateCloud(), new HybridHostingType(), null, false },
            new object[] { new PublicCloud(), new PrivateCloud(), new HybridHostingType(), new OnPremise(), false },
            new object[] { new PublicCloud { Summary = "Test " }, null, null, null, true },
            new object[] { null, new PrivateCloud { HostingModel = "Test", Summary = "Test" }, null, null, true },
            new object[]
            {
                null, null, new HybridHostingType { HostingModel = "Test", Summary = "Test" }, null, true,
            },
            new object[] { null, null, null, new OnPremise { HostingModel = "Test", Summary = "Test" }, true },
        };

        [Fact]
        public static void PublicCloud_IsValid_False()
        {
            new PublicCloud().IsValid().Should().BeFalse();
        }

        [Fact]
        public static void PrivateCloud_IsValid_False()
        {
            new PrivateCloud().IsValid().Should().BeFalse();
        }

        [Fact]
        public static void Hybrid_IsValid_False()
        {
            new HybridHostingType().IsValid().Should().BeFalse();
        }

        [Fact]
        public static void OnPremise_IsValid_False()
        {
            new OnPremise().IsValid().Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void PublicCloud_IsValid_True(PublicCloud publicCloud)
        {
            publicCloud.IsValid().Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void PrivateCloud_IsValid_True(PrivateCloud privateCloud)
        {
            privateCloud.IsValid().Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void Hybrid_IsValid_True(HybridHostingType hybrid)
        {
            hybrid.IsValid().Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void OnPremise_IsValid_True(OnPremise onPremise)
        {
            onPremise.IsValid().Should().BeTrue();
        }

        [Fact]
        public static void New_Hosting()
        {
            var hosting = new Hosting();

            hosting.PublicCloud.Should().NotBeNull();
            hosting.PrivateCloud.Should().NotBeNull();
            hosting.HybridHostingType.Should().NotBeNull();
            hosting.OnPremise.Should().NotBeNull();
        }

        [Theory]
        [MockMemberAutoData(nameof(HostingTestData))]
        public static void Hosting_IsValid_True(
            PublicCloud publicCloud,
            PrivateCloud privateCloud,
            HybridHostingType hybrid,
            OnPremise onPremise,
            bool expected)
        {
            var hosting = new Hosting
            {
                PublicCloud = publicCloud,
                PrivateCloud = privateCloud,
                HybridHostingType = hybrid,
                OnPremise = onPremise,
            };

            hosting.IsValid().Should().Be(expected);
        }
    }
}
