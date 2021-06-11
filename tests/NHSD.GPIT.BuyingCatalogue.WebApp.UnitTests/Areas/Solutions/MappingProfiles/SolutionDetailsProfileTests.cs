using System;
using System.Collections.Generic;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models;
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
                cfg.CreateMap<CatalogueItem, TestSolutionDisplayBaseModel>()
                    .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>()
                    .ForAllOtherMembers(opt => opt.Ignore());
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

        [Test]
        public void Map_CatalogueItemToSolutionDisplayBaseModel_ShowFunctionsCalled()
        {
            var mockCatalogueItem = new Mock<CatalogueItem>();
            
            mapper.Map<CatalogueItem, TestSolutionDisplayBaseModel>(mockCatalogueItem.Object);
            
            mockCatalogueItem.Verify(c => c.HasFeatures());
            mockCatalogueItem.Verify(c => c.HasCapabilities());
            mockCatalogueItem.Verify(c => c.HasListPrice());
            mockCatalogueItem.Verify(c => c.HasAdditionalServices());
            mockCatalogueItem.Verify(c => c.HasAssociatedServices());
            mockCatalogueItem.Verify(c => c.HasInteroperability());
            mockCatalogueItem.Verify(c => c.HasImplementationDetail());
            mockCatalogueItem.Verify(c => c.HasClientApplication());
            mockCatalogueItem.Verify(c => c.HasHosting());
            mockCatalogueItem.Verify(c => c.HasServiceLevelAgreement());
            mockCatalogueItem.Verify(c => c.HasDevelopmentPlans());
            mockCatalogueItem.Verify(c => c.HasSupplierDetails());
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
                    Action = "ClientApplicationTypes",
                    Controller = "SolutionDetails",
                    Name = "Client application type",
                    Show = true,
                },
                Previous = new SectionModel
                {
                    Action = "Description",
                    Controller = "SolutionDetails",
                    Name = "Interoperability",
                    Show = true,
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
                    Show = true,
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
                    Show = true,
                },
                //TODO: Update Next to Capabilities once Capabilities page implemented
                Next = new SectionModel
                {
                    Action = "Capabilities",
                    Controller = "SolutionDetails",
                    Name = "Capabilities",
                    Show = true,
                },
            });
            actual.Section.Should().Be("Features");
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SolutionName.Should().Be(catalogueItem.Name);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToHostingTypesModel_ResultAsExpected(
           CatalogueItem catalogueItem)
        {
            var actual = mapper.Map<CatalogueItem, HostingTypesModel>(catalogueItem);

            configuration.Verify(c => c["SolutionsLastReviewedDate"]);
            actual.LastReviewed.Should().Be(LastReviewedDate);
            actual.PaginationFooter.Should().BeEquivalentTo(new PaginationFooterModel
            {
                Previous = new SectionModel
                {
                    Action = "ClientApplicationTypes",
                    Controller = "SolutionDetails",
                    Name = "Client application type",
                    Show = true,
                },

                Next = new SectionModel
                {
                    Action = "Description",
                    Controller = "SolutionDetails",
                    Name = "Service Level Agreement",
                    Show = true,
                },
            });
            actual.Section.Should().Be("Hosting type");
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SolutionName.Should().Be(catalogueItem.Name);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToClientApplicationTypesModel_ResultAsExpected(
           CatalogueItem catalogueItem)
        {
            var actual = mapper.Map<CatalogueItem, ClientApplicationTypesModel>(catalogueItem);

            configuration.Verify(c => c["SolutionsLastReviewedDate"]);

            actual.LastReviewed.Should().Be(LastReviewedDate);
            actual.PaginationFooter.Should().BeEquivalentTo(new PaginationFooterModel
            {
                Previous = new SectionModel
                {
                    Action = "ImplementationTimescales",
                    Controller = "SolutionDetails",
                    Name = "Implementation timescales",
                    Show = true,
                },
                Next = new SectionModel
                {
                    Action = "HostingType",
                    Controller = "SolutionDetails",
                    Name = "Hosting type",
                    Show = true,
                },
            });
            actual.Section.Should().Be("Client application type");
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SolutionName.Should().Be(catalogueItem.Name);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToCapabilitiesViewModel_ResultAsExpected(
           CatalogueItem catalogueItem)
        {
            var actual = mapper.Map<CatalogueItem, CapabilitiesViewModel>(catalogueItem);

            configuration.Verify(c => c["SolutionsLastReviewedDate"]);

            actual.LastReviewed.Should().Be(LastReviewedDate);
            actual.PaginationFooter.Should().BeEquivalentTo(new PaginationFooterModel
            {
                Previous = new SectionModel
                {
                    Action = "Features",
                    Controller = "SolutionDetails",
                    Name = "Features",
                    Show = true,
                },
                //TODO: Update Next to List price once List price page implemented
                Next = new SectionModel
                {
                    Action = "Description",
                    Controller = "SolutionDetails",
                    Name = "List price",
                    Show = true,
                },
            });
            actual.Section.Should().Be("Capabilities");
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
        public void Map_CatalogueItemToSolutionDescriptionModel_SetsFrameworkAsExpected(List<string> expected)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>();
            mockCatalogueItem.Setup(c => c.Frameworks())
                .Returns(expected);

            var actual = mapper.Map<CatalogueItem, SolutionDescriptionModel>(mockCatalogueItem.Object);

            mockCatalogueItem.Verify(c => c.Frameworks());
            actual.Frameworks.Should().BeEquivalentTo(expected);
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
