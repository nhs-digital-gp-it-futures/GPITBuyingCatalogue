using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.MappingProfiles
{
    public sealed class IntegrationProfileTests : IDisposable
    {
        private const string LastReviewedDate = "26 Aug 2025";

        private readonly Mock<IMemberValueResolver<CatalogueItem, InteroperabilityModel, string, IList<IntegrationModel>>> integrationsResolver;

        private IMapper mapper;
        private Mock<IConfiguration> configuration;
        private MapperConfiguration mapperConfiguration;

        public IntegrationProfileTests()
        {
            configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["SolutionsLastReviewedDate"])
                .Returns(LastReviewedDate);

            integrationsResolver =
                new Mock<IMemberValueResolver<CatalogueItem, InteroperabilityModel, string, IList<IntegrationModel>>>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(
                    s => s.GetService(typeof(IMemberValueResolver<object, object, string, string>)))
                .Returns(new ConfigSettingResolver(configuration.Object));
            serviceProvider.Setup(
                    s => s.GetService(typeof(IMemberValueResolver<CatalogueItem, InteroperabilityModel, string, IList<IntegrationModel>>)))
                .Returns(integrationsResolver.Object);
            mapperConfiguration = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<IntegrationProfile>();
                    cfg.AddProfile<SolutionDetailsProfile>();
                });

            mapper = mapperConfiguration.CreateMapper(serviceProvider.Object.GetService);
        }

        public void Dispose()
        {
            configuration = null;
            mapperConfiguration = null;
            mapper = null;
        }

        [Theory]
        [CommonAutoData]
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
                            Action = nameof(SolutionDetailsController.Implementation),
                            Controller = typeof(SolutionDetailsController).ControllerName(),
                            Name = "Implementation",
                            Show = true,
                        },
                        Previous = new SectionModel
                        {
                            Action = nameof(SolutionDetailsController.AssociatedServices),
                            Controller = typeof(SolutionDetailsController).ControllerName(),
                            Name = "Associated Services",
                            Show = true,
                        },
                    });
            actual.Section.Should().Be("Interoperability");
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SolutionName.Should().Be(catalogueItem.Name);
        }

        [Theory]
        [CommonAutoData]
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
