using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.MappingProfiles
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class IntegrationProfileTests
    {
        private IMapper mapper;
        private Mock<IConfiguration> configuration;
        private MapperConfiguration mapperConfiguration;
        private Mock<IMemberValueResolver<CatalogueItem, InteroperabilityModel, string,
            IList<IntegrationModel>>> integrationsResolver;
        private const string LastReviewedDate = "26 Aug 2025";

        [OneTimeSetUp]
        public void SetUp()
        {
            configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["SolutionsLastReviewedDate"])
                .Returns(LastReviewedDate);

            integrationsResolver =
                new Mock<IMemberValueResolver<CatalogueItem, InteroperabilityModel, string, IList<IntegrationModel>>>();
            
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(
                    x =>
                        x.GetService(typeof(IMemberValueResolver<object, object, string, string>)))
                .Returns(new ConfigSettingResolver(configuration.Object));
            serviceProvider.Setup(
                    x =>
                        x.GetService(typeof(IMemberValueResolver<CatalogueItem, InteroperabilityModel, string,
                            IList<IntegrationModel>>)))
                .Returns(integrationsResolver.Object);
            mapperConfiguration = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<IntegrationProfile>();
                    cfg.AddProfile<SolutionDetailsProfile>();
                });
            
            mapper = mapperConfiguration.CreateMapper(serviceProvider.Object.GetService);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            configuration = null;
            mapperConfiguration = null;
            mapper = null;
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToInteroperabilityModel_ResultAsExpected(CatalogueItem catalogueItem)
        {
            var actual = mapper.Map<CatalogueItem, InteroperabilityModel>(catalogueItem);
            
            configuration.Verify(c => c["SolutionsLastReviewedDate"]);
            actual.LastReviewed.Should().Be(LastReviewedDate);
            actual.PaginationFooter.Should()
                .BeEquivalentTo(
                    new PaginationFooterModel
                    {
                        FullWidth = true,
                        Next = new SectionModel
                        {
                            Action = nameof(SolutionDetailsController.ImplementationTimescales),
                            Controller = typeof(SolutionDetailsController).ControllerName(),
                            Name = "Implementation",
                            Show = true,
                        },
                        Previous = new SectionModel
                        {
                            Action = nameof(SolutionDetailsController.Description),
                            Controller = typeof(SolutionDetailsController).ControllerName(),
                            Name = "Associated Services",
                            Show = true,
                        },
                    });
            actual.Section.Should().Be("Interoperability");
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SolutionName.Should().Be(catalogueItem.Name);
        }
        
        [Test, CommonAutoData]
        public void Map_CatalogueItemToInteroperabilityModel_SetsIntegrationModelsFromResolver(
            CatalogueItem catalogueItem, 
            List<IntegrationModel> integrationModels)
        {
            integrationsResolver.Setup(
                    r => r.Resolve(
                        It.IsAny<CatalogueItem>(),
                        It.IsAny<InteroperabilityModel>(),
                        catalogueItem.Solution.Integrations,
                        It.IsAny<IList<IntegrationModel>>(),
                        It.IsAny<ResolutionContext>()))
                .Returns(integrationModels);

            var actual = mapper.Map<CatalogueItem, InteroperabilityModel>(catalogueItem);
            
            integrationsResolver.Verify(
                    r => r.Resolve(
                        It.IsAny<CatalogueItem>(),
                        It.IsAny<InteroperabilityModel>(),
                        catalogueItem.Solution.Integrations,
                        It.IsAny<IList<IntegrationModel>>(),
                        It.IsAny<ResolutionContext>()));
            actual.Integrations.Should().BeEquivalentTo(integrationModels);
        }
    }
}
