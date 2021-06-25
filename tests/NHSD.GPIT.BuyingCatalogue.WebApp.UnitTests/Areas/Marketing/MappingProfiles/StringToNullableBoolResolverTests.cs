using System;
using AutoMapper;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.MappingProfiles
{
    // TODO: build action set to none to ignore tests as they will fail until rewritten without the setup method
    public static class StringToNullableBoolResolverTests
    {
        private static IMapper _mapper;

        // TODO: no setup methods in xUnit
        public static void SetUp()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x =>
                    x.GetService(typeof(IMemberValueResolver<object, object, string, bool?>)))
                .Returns(new StringToNullableBoolResolver());
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper(serviceProvider.Object.GetService);
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Resolve_SourceMemberIsInvalidString_ReturnsNull(string invalid)
        {
            var model = new SupportedBrowsersModel
            {
                MobileResponsive = invalid,
            };

            var actual = _mapper.Map<SupportedBrowsersModel, ClientApplication>(model);

            actual.MobileResponsive.Should().BeNull();
        }

        [Theory]
        [InlineData("YES")]
        [InlineData("yes")]
        [InlineData("Yes")]
        public static void Resolve_SourceMemberIsYes_ReturnsTrue(string input)
        {
            var model = new SupportedBrowsersModel
            {
                MobileResponsive = input,
            };

            var actual = _mapper.Map<SupportedBrowsersModel, ClientApplication>(model);

            actual.MobileResponsive.Should().BeTrue();
        }

        [Theory]
        [InlineData("NO")]
        [InlineData("No")]
        [InlineData("no")]
        [InlineData("abc")]
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
