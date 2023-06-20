using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    public static class HostingTypesTests
    {
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

        [Fact]
        public static void Hosting_IsValid_False()
        {
            new Hosting().IsValid().Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void PublicCloud_IsValid_True(PublicCloud publicCloud)
        {
            publicCloud.IsValid().Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void PrivateCloud_IsValid_True(PrivateCloud privateCloud)
        {
            privateCloud.IsValid().Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void Hybrid_IsValid_True(HybridHostingType hybrid)
        {
            hybrid.IsValid().Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static void Hosting_IsValid_True(
            PublicCloud publicCloud,
            PrivateCloud privateCloud,
            HybridHostingType hybrid,
            OnPremise onPremise)
        {
            var hosting = new Hosting
            {
                PublicCloud = publicCloud,
                PrivateCloud = privateCloud,
                HybridHostingType = hybrid,
                OnPremise = onPremise,
            };

            hosting.IsValid().Should().BeTrue();
        }
    }
}
