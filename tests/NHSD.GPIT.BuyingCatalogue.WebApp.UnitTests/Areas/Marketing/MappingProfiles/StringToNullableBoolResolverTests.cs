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
    public class StringToNullableBoolResolverTests
    {
        private readonly IMapper mapper;

        public StringToNullableBoolResolverTests()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x =>
                    x.GetService(typeof(IMemberValueResolver<object, object, string, bool?>)))
                .Returns(new StringToNullableBoolResolver());
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper(serviceProvider.Object.GetService);
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public void Resolve_SourceMemberIsInvalidString_ReturnsNull(string invalid)
        {
            var model = new SupportedBrowsersModel
            {
                MobileResponsive = invalid,
            };

            var actual = mapper.Map<SupportedBrowsersModel, ClientApplication>(model);

            actual.MobileResponsive.Should().BeNull();
        }

        [Theory]
        [InlineData("YES")]
        [InlineData("yes")]
        [InlineData("Yes")]
        public void Resolve_SourceMemberIsYes_ReturnsTrue(string input)
        {
            var model = new SupportedBrowsersModel
            {
                MobileResponsive = input,
            };

            var actual = mapper.Map<SupportedBrowsersModel, ClientApplication>(model);

            actual.MobileResponsive.Should().BeTrue();
        }

        [Theory]
        [InlineData("NO")]
        [InlineData("No")]
        [InlineData("no")]
        [InlineData("abc")]
        public void Resolve_SourceMemberIsNotYes_ReturnsFalse(string input)
        {
            var model = new SupportedBrowsersModel
            {
                MobileResponsive = input,
            };

            var actual = mapper.Map<SupportedBrowsersModel, ClientApplication>(model);

            actual.MobileResponsive.Should().BeFalse();
        }
    }
}
