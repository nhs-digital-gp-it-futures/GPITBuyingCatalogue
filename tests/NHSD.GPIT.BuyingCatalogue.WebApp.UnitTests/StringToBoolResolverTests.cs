using System;
using AutoMapper;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests
{
    internal static class StringToBoolResolverTests
    {
        private static string[] InvalidStrings = { null, string.Empty, "    " };
        private static IMapper _mapper;

        [SetUp]
        public static void SetUp()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x =>
                    x.GetService(typeof(IMemberValueResolver<object, object, string, bool?>)))
                .Returns(new StringToBoolResolver());
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper(serviceProvider.Object.GetService);
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Resolve_SourceMemberIsInvalidString_ReturnsNull(string invalid)
        {
            var model = new SupportedBrowsersModel
            {
                MobileResponsive = invalid,
            };

            var actual = _mapper.Map<SupportedBrowsersModel, ClientApplication>(model);

            actual.MobileResponsive.Should().BeNull();
        }
        
        [Test]
        [TestCase("YES")]
        [TestCase("yes")]
        [TestCase("Yes")]
        public static void Resolve_SourceMemberIsYes_ReturnsTrue(string input)
        {
            var model = new SupportedBrowsersModel
            {
                MobileResponsive = input,
            };

            var actual = _mapper.Map<SupportedBrowsersModel, ClientApplication>(model);

            actual.MobileResponsive.Should().BeTrue();
        }

        [Test]
        [TestCase("NO")]
        [TestCase("No")]
        [TestCase("no")]
        [TestCase("abc")]
        public static void Resolve_SourceMemberIsNotYes_ReturnsFalse(string input)
        {
            var model = new SupportedBrowsersModel
            {
                MobileResponsive = input,
            };

            var actual = _mapper.Map<SupportedBrowsersModel, ClientApplication>(model);

            actual.MobileResponsive.Should().BeFalse();
        }
    }
}