using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.MappingProfiles
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class IntegrationModelResolverTests
    {
        private static IMapper mapper;

        [OneTimeSetUp]
        public void SetUp()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<IntegrationProfile>();
                cfg.AddProfile<SolutionDetailsProfile>();
            }).CreateMapper(serviceProvider.Object.GetService);
            serviceProvider.Setup(
                    x =>
                        x.GetService(
                            typeof(IMemberValueResolver<CatalogueItem, InteroperabilityModel, string,
                                IList<IntegrationModel>>)))
                .Returns(new IntegrationModelsResolver(mapper));
            serviceProvider.Setup(x => x.GetService(typeof(IMemberValueResolver<object, object, string, string>)))
                .Returns(new Mock<IMemberValueResolver<object, object, string, string>>().Object);
        }
        
        [OneTimeTearDown]
        public void CleanUp()
        {
            mapper = null;
        }

        [Test, CommonAutoData]
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
                            .BeEquivalentTo("Provider or Consumer", "Additional information");
                        integrationModel.Tables[inner]
                            .Rows.Should()
                            .BeEquivalentTo(
                                integration.SubTypes[inner].DetailsDictionary.Select(d => new[] { d.Key, d.Value }));
                    }
                    else
                    {
                        integrationModel.Tables[inner]
                            .Headings.Should()
                            .BeEquivalentTo("Provider or Consumer", "System integrating with", "Description");
                        integrationModel.Tables[inner]
                            .Rows.Should()
                            .BeEquivalentTo(
                                integration.SubTypes[inner].DetailsSystemDictionary.SelectMany(GetRows));
                    }
                }
            }
        }

        [Test, CommonAutoData]
        public void Map_IntegrationsToIntegrationModels_NoIntegrations_ReturnsEmptyList(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.Integrations = JsonConvert.SerializeObject(new List<Integration>());

            var actual = mapper.Map<CatalogueItem, InteroperabilityModel>(catalogueItem);

            actual.Integrations.Should().BeEmpty();
        }
        
        [Test, CommonAutoData]
        public void Map_IntegrationsToIntegrationModels_InvalidIntegrations_ReturnsEmptyList(CatalogueItem catalogueItem)
        {
            new List<string>{null, "", "    "}
                .ForEach(
                    invalid =>
                    {
                        catalogueItem.Solution.Integrations = invalid;

                        var actual = mapper.Map<CatalogueItem, InteroperabilityModel>(catalogueItem);

                        actual.Integrations.Should().BeEmpty();
                    });
        }

        private static IEnumerable<string[]> GetRows(KeyValuePair<string,Dictionary<string,string>> keyValuePair)
        {
            var result = new List<string[]>();

            foreach (var (key, value) in keyValuePair.Value)
            {
                result.Add(new []{keyValuePair.Key, key, value});
            }

            return result;
        }
    }
}
