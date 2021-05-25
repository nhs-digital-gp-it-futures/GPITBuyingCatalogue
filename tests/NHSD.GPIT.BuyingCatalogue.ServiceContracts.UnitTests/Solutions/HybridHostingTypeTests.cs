using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Solutions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HybridHostingTypeTests
    {
        [Test]
        public static void Link_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(HybridHostingType)
                .GetProperty(nameof(HybridHostingType.Link))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(1000);
        }

        [Test]
        public static void Link_UrlAttribute_Present()
        {
            typeof(HybridHostingType)
                .GetProperty(nameof(HybridHostingType.Link))
                .GetCustomAttribute<UrlAttribute>()
                .Should().NotBeNull();
        }
        
        [Test]
        public static void HostingModel_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(HybridHostingType)
                .GetProperty(nameof(HybridHostingType.HostingModel))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(1000);
        }
        
        [Test]
        public static void Summary_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(HybridHostingType)
                .GetProperty(nameof(HybridHostingType.Summary))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(500);
        }

        [Test]
        public static void IsValid_LinkHasValue_ReturnsTrue()
        {
            var model = new HybridHostingType { Link = "some-value", };

            var actual = model.IsValid();
            
            actual.Should().BeTrue();
        }
        
        [Test]
        public static void IsValid_HostingModelHasValue_ReturnsTrue()
        {
            var model = new HybridHostingType { HostingModel = "some-value", };

            var actual = model.IsValid();
            
            actual.Should().BeTrue();
        }
        
        [Test]
        public static void IsValid_RequiresHscnHasValue_ReturnsTrue()
        {
            var model = new HybridHostingType { RequiresHscn = "some-value", };

            var actual = model.IsValid();
            
            actual.Should().BeTrue();
        }

        [Test]
        public static void IsValid_SummaryHasValue_ReturnsTrue()
        {
            var model = new HybridHostingType { Summary = "some-value", };

            var actual = model.IsValid();
            
            actual.Should().BeTrue();
        }

        [Test]
        public static void IsValid_NoPropertyHasValue_ReturnsFalse()
        {
            var model = new HybridHostingType();

            var actual = model.IsValid();
            
            actual.Should().BeFalse();
        }
    }
}
