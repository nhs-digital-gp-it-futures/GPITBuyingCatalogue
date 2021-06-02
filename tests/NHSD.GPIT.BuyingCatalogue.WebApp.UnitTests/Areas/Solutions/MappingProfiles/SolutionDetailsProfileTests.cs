using System;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.MappingProfiles
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class SolutionDetailsProfileTests
    {
        private IMapper mapper;
        private Mock<IConfiguration> configuration;
        private MapperConfiguration mapperConfiguration;
        private const string LastReviewedDate = "26 Aug 2025";

        [OneTimeSetUp]
        public void SetUp()
        {
            configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["SolutionsLastReviewedDate"])
                .Returns(LastReviewedDate);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x =>
                    x.GetService(typeof(IMemberValueResolver<object, object, string, string>)))
                .Returns(new ConfigSettingResolver(configuration.Object));
            mapperConfiguration = new MapperConfiguration(cfg =>
            {
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

        [Test]
        public void Mapper_Configuration_Valid()
        {
            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToImplementationTimescalesModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var actual = mapper.Map<CatalogueItem, ImplementationTimescalesModel>(catalogueItem);
            
            configuration.Verify(c => c["SolutionsLastReviewedDate"]);
            actual.Description.Should().Be(catalogueItem.Solution.ImplementationDetail);
            actual.LastReviewed.Should().Be(LastReviewedDate);
            actual.PaginationFooter.Should().BeEquivalentTo(new PaginationFooterModel
            {
                Next = new SectionModel
                {
                    Action = "Description",
                    Controller = "SolutionDetails",
                    Name = "Client application type",
                },
                Previous = new SectionModel
                {
                    Action = "Description",
                    Controller = "SolutionDetails",
                    Name = "Interoperability",
                },
            });
            actual.Section.Should().Be("Implementation timescales");
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SolutionName.Should().Be(catalogueItem.Name);
        }
        
        [Test, CommonAutoData]
        public void Map_CatalogueItemToSolutionDescriptionModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var actual = mapper.Map<CatalogueItem, SolutionDescriptionModel>(catalogueItem);

            configuration.Verify(c => c["SolutionsLastReviewedDate"]);
            actual.Description.Should().Be(catalogueItem.Solution.FullDescription);
            actual.LastReviewed.Should().Be(LastReviewedDate);
            actual.PaginationFooter.Should().BeEquivalentTo(new PaginationFooterModel
            {
                Next = new SectionModel
                {
                    Action = "Features",
                    Controller = "SolutionDetails",
                    Name = "Features",
                },
            });
            actual.Section.Should().Be("Description");
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SolutionName.Should().Be(catalogueItem.Name);
            actual.Summary.Should().Be(catalogueItem.Solution.Summary);
            actual.SupplierName.Should().Be(catalogueItem.Supplier.Name);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToSolutionFeaturesModel_ResultAsExpected(
           CatalogueItem catalogueItem)
        {
            var actual = mapper.Map<CatalogueItem, SolutionFeaturesModel>(catalogueItem);

            configuration.Verify(c => c["SolutionsLastReviewedDate"]);
            var features = JsonConvert.DeserializeObject<string[]>(catalogueItem.Solution.Features);
            
            actual.Features.Should().BeEquivalentTo(features);
            actual.LastReviewed.Should().Be(LastReviewedDate);
            actual.PaginationFooter.Should().BeEquivalentTo(new PaginationFooterModel
            {
                Previous = new SectionModel
                {
                    Action = "Description",
                    Controller = "SolutionDetails",
                    Name = "Description",
                },
                //TODO: Update Next to Capabilities once Capabilities page implemented
                Next = new SectionModel
                {
                    Action = "Description",
                    Controller = "SolutionDetails",
                    Name = "Capabilities",
                },
            });
            actual.Section.Should().Be("Features");
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SolutionName.Should().Be(catalogueItem.Name);
        }

        [TestCase(false, "No")]
        [TestCase(null, "")]
        [TestCase(true, "Yes")]
        public void Map_CatalogueItemToSolutionDescriptionModel_SetsIsFoundationAsExpected(
            bool? isFoundation,
            string expected)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>();
            mockCatalogueItem.Setup(c => c.IsFoundation())
                .Returns(isFoundation);

            var actual = mapper.Map<CatalogueItem, SolutionDescriptionModel>(mockCatalogueItem.Object);

            mockCatalogueItem.Verify(c => c.IsFoundation());
            actual.IsFoundation.Should().Be(expected);
        }
        
        [AutoData]
        [Test]
        public void Map_CatalogueItemToSolutionDescriptionModel_SetsFrameworkAsExpected(string expected)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>();
            mockCatalogueItem.Setup(c => c.Framework())
                .Returns(expected);

            var actual = mapper.Map<CatalogueItem, SolutionDescriptionModel>(mockCatalogueItem.Object);

            mockCatalogueItem.Verify(c => c.Framework());
            actual.Framework.Should().Be(expected);
        }

        [AutoData]
        [Test]
        public void Map_CatalogueItemToSolutionFeaturesModel_SetsFeaturesAsExpected(string[] expected)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>();
            mockCatalogueItem.Setup(c => c.Features())
                .Returns(expected);

            var actual = mapper.Map<CatalogueItem, SolutionFeaturesModel>(mockCatalogueItem.Object);

            mockCatalogueItem.Verify(c => c.Features());
            actual.Features.Should().BeEquivalentTo(expected);
        }
    }
}
