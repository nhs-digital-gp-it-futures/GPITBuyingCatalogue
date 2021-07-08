using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.MappingProfiles
{
    public sealed class IntegrationModelResolverTests : IDisposable
    {
        private static IMapper mapper;

        public IntegrationModelResolverTests()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            mapper = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<IntegrationProfile>();
                    cfg.AddProfile<SolutionDetailsProfile>();
                }).CreateMapper(serviceProvider.Object.GetService);
            serviceProvider.Setup(
                    s =>
                        s.GetService(
                            typeof(IMemberValueResolver<CatalogueItem, InteroperabilityModel, string,
                                IList<IntegrationModel>>)))
                .Returns(new IntegrationModelsResolver(mapper));
            serviceProvider.Setup(s => s.GetService(typeof(IMemberValueResolver<object, object, string, string>)))
                .Returns(new Mock<IMemberValueResolver<object, object, string, string>>().Object);
        }

        public void Dispose()
        {
            mapper = null;
        }

        [Theory]
        [CommonAutoData]
        public void Map_IntegrationsToIntegrationModels_ResultAsExpected(CatalogueItem catalogueItem)
        {
            var integrations = JsonConvert.DeserializeObject<List<Integration>>(catalogueItem.Solution.Integrations);
            integrations.Count.Should().BeGreaterThan(1);

            var actual = mapper.Map<CatalogueItem, InteroperabilityModel>(catalogueItem).Integrations;

            actual.Count.Should().Be(integrations.Count);
            for (int outer = 0; outer < actual.Count; outer++)
            {
                var integrationModel = actual[outer];
                var integration = integrations[outer];

                integrationModel.Name.Should().Be(integration.Name);

                for (int inner = 0; inner < integration.SubTypes.Length; inner++)
                {
                    integrationModel.Tables[inner].Name.Should().Be(integration.SubTypes[inner].Name);
                    if (integration.SubTypes[inner].DetailsDictionary.Any())
                    {
                        integrationModel.Tables[inner]
                            .Headings.Should()
                            .BeEquivalentTo("Provider or consumer", "Additional information");
                        integrationModel.Tables[inner]
                            .Rows.Should()
                            .BeEquivalentTo(
                                integration.SubTypes[inner].DetailsDictionary.Select(d => new[] { d.Key, d.Value }));
                    }
                    else
                    {
                        integrationModel.Tables[inner]
                            .Headings.Should()
                            .BeEquivalentTo("Provider or consumer", "System integrating with", "Description");
                        integrationModel.Tables[inner]
                            .Rows.Should()
                            .BeEquivalentTo(
                                integration.SubTypes[inner].DetailsSystemDictionary.SelectMany(GetRows));
                    }
                }
            }
        }

        [Theory]
        [CommonAutoData]
        public void Map_IntegrationsToIntegrationModels_NoIntegrations_ReturnsEmptyList(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.Integrations = JsonConvert.SerializeObject(new List<Integration>());

            var actual = mapper.Map<CatalogueItem, InteroperabilityModel>(catalogueItem);

            actual.Integrations.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public void Map_IntegrationsToIntegrationModels_InvalidIntegrations_ReturnsEmptyList(CatalogueItem catalogueItem)
        {
            new List<string> { null, string.Empty, "    " }
                .ForEach(
                    invalid =>
                    {
                        catalogueItem.Solution.Integrations = invalid;

                        var actual = mapper.Map<CatalogueItem, InteroperabilityModel>(catalogueItem);

                        actual.Integrations.Should().BeEmpty();
                    });
        }

        private static IEnumerable<string[]> GetRows(KeyValuePair<string, Dictionary<string, string>> keyValuePair)
        {
            var result = new List<string[]>();

            foreach (var (key, value) in keyValuePair.Value)
            {
                result.Add(new[] { keyValuePair.Key, key, value });
            }

            return result;
        }
    }
}
