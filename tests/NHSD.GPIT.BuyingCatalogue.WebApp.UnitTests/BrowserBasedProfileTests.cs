using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class BrowserBasedProfileTests
    {
        [Test]
        public static void Mappings_Configuration_Valid()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            });

            mapperConfiguration.AssertConfigurationIsValid();
        }
        
        [Test, IgnoreCircularReferenceAutoData]
        public static void Map_CatalogueItemToSupportedBrowsersModel_ResultAsExpected(CatalogueItem catalogueItem,
            ClientApplication clientApplication)
        {
            clientApplication.BrowsersSupported = new HashSet<string>
                {"Internet Explorer 11", "Google Chrome", "OPERA", "safari", "mozilla firefox"};
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);
            
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x =>
                    x.GetService(typeof(IMemberValueResolver<object, object, string, bool?>)))
                .Returns(new Mock<IMemberValueResolver<object, object, string, bool?>>().Object);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper(serviceProvider.Object.GetService);

            var actual = mapper.Map<CatalogueItem, SupportedBrowsersModel>(catalogueItem);

            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/browser-based");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.Browsers.Should().BeEquivalentTo(new SupportedBrowserModel[]
            {
                new() {BrowserName = "Google Chrome", Checked = true, },
                new() {BrowserName = "Microsoft Edge", Checked = false, },
                new() {BrowserName = "Mozilla Firefox", Checked = true, },
                new() {BrowserName = "Opera", Checked = true, },
                new() {BrowserName = "Safari", Checked = true, },
                new() {BrowserName = "Chromium", Checked = false, },
                new() {BrowserName = "Internet Explorer 11", Checked = true, },
                new() {BrowserName = "Internet Explorer 10", Checked = false, }
            });
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.MobileResponsive.Should().Be(clientApplication.MobileResponsive.ToYesNo());
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, IgnoreCircularReferenceAutoData]
        public static void Map_SupportedBrowsersModelToClientApplication_ResultAsExpected(SupportedBrowsersModel model,
            ClientApplication clientApplication)
        {
            var original = new ClientApplication();
            var propInfo = clientApplication.GetType().GetProperties();
            foreach (var item in propInfo)
            {
                original.GetType().GetProperty(item.Name).SetValue(original,
                    item.GetValue(clientApplication, null), null);
            }
            
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x =>
                    x.GetService(typeof(IMemberValueResolver<object, object, string, bool?>)))
                .Returns(new Mock<IMemberValueResolver<object, object, string, bool?>>().Object);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper(serviceProvider.Object.GetService);

            var actual = mapper.Map(model, clientApplication);

            actual.BrowsersSupported.Should()
                .BeEquivalentTo(model.Browsers.Where(x => x.Checked).Select(x => x.BrowserName));
            
            var remainingProperties = original.GetType().GetProperties()
                .Where(x => !new[] {"BrowsersSupported", "MobileResponsive"}.Contains(x.Name));

            foreach (var item in remainingProperties)
            {
                actual.GetType().GetProperty(item.Name).GetValue(actual).Should()
                    .BeEquivalentTo(typeof(ClientApplication).GetProperty(item.Name).GetValue(original));
            }
        }

        [Test, AutoData]
        public static void Map_SupportedBrowsersModelToClientApplication_SetsMobileResponsiveFromResolver(
            SupportedBrowsersModel model)
        {
            var clientApplication = new ClientApplication();
            var expected = DateTime.Now.Ticks % 2 == 0;
            var mobileResponsiveResolver = new Mock<IMemberValueResolver<object, object, string, bool?>>();
            mobileResponsiveResolver.Setup(x => x.Resolve(It.IsAny<object>(), It.IsAny<object>(),
                    model.MobileResponsive, It.IsAny<bool?>(), It.IsAny<ResolutionContext>()))
                .Returns(expected);
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x =>
                    x.GetService(typeof(IMemberValueResolver<object, object, string, bool?>)))
                .Returns(mobileResponsiveResolver.Object);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper(serviceProvider.Object.GetService);

            mapper.Map(model, clientApplication);

            mobileResponsiveResolver.Verify(x => x.Resolve(It.IsAny<object>(), It.IsAny<object>(),
                model.MobileResponsive, It.IsAny<bool?>(), It.IsAny<ResolutionContext>()));
            clientApplication.MobileResponsive.Should().Be(expected);
        }
    }
}