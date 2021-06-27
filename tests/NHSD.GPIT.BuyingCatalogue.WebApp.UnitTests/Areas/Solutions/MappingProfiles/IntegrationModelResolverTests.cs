using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Bogus.Extensions;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.MappingProfiles
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class IntegrationModelResolverTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };
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

        [Test]
        public void Map_IntegrationsToIntegrationModels_ResultAsExpected()
        {
            var integrations = Fakers.Integration.GenerateBetween(4, 9);

            var actual = mapper.Map<CatalogueItem, InteroperabilityModel>(new CatalogueItem
            {
                Solution = new Solution{ Integrations = JsonConvert.SerializeObject(integrations),}
            }).Integrations;

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
                            .BeEquivalentTo("Provider or Consumer", "Integrates with system", "Description");
                        integrationModel.Tables[inner]
                            .Rows.Should()
                            .BeEquivalentTo(
                                integration.SubTypes[inner].DetailsSystemDictionary.SelectMany(GetRows));
                    }
                }
            }
        }

        [Test]
        public void Map_IntegrationsToIntegrationModels_NoIntegrations_ReturnsEmptyList()
        {
            var actual = mapper.Map<CatalogueItem, InteroperabilityModel>(new CatalogueItem
            {
                Solution = new Solution{ Integrations = JsonConvert.SerializeObject(new List<Integration>()),}
            });

            actual.Integrations.Should().BeEmpty();
        }
        
        [TestCaseSource(nameof(InvalidStrings))]
        public void Map_IntegrationsToIntegrationModels_InvalidIntegrations_ReturnsEmptyList(string invalid)
        {
            var catalogueItem = new CatalogueItem
            {
                Solution = new Solution { Integrations = invalid, }
            };

            var actual = mapper.Map<CatalogueItem, InteroperabilityModel>(catalogueItem);

            actual.Integrations.Should().BeEmpty();
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
