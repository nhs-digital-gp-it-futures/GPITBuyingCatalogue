﻿using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.MappingProfiles
{
    public sealed class SolutionsProfileTests
    {
        private readonly IMapper mapper;
        private readonly MapperConfiguration mapperConfiguration;

        public SolutionsProfileTests()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            mapperConfiguration = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<SolutionsProfile>();
                    cfg.CreateMap<CatalogueItem, TestSolutionDisplayBaseModel>()
                        .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>()
                        .ForAllOtherMembers(opt => opt.Ignore());
                });
            mapper = mapperConfiguration.CreateMapper(serviceProvider.Object.GetService);
        }

        [Fact]
        public void Mapper_Configuration_Valid()
        {
            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Theory]
        [CommonAutoData]
        public void Map_CatalogueItemToHostingTypesModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var actual = mapper.Map<CatalogueItem, HostingTypesModel>(catalogueItem);
            actual.PaginationFooter.Should()
                .BeEquivalentTo(
                    new PaginationFooterModel
                    {
                        Previous = new SectionModel
                        {
                            Action = "ClientApplicationTypes",
                            Controller = "Solutions",
                            Name = "Client application type",
                            Show = true,
                        },
                        Next = new SectionModel
                        {
                            Action = "Description",
                            Controller = "Solutions",
                            Name = "Service Level Agreement",
                            Show = true,
                        },
                    });
            actual.Section.Should().Be("Hosting type");
            actual.SolutionId.Should().Be(catalogueItem.Id);
            actual.SolutionName.Should().Be(catalogueItem.Name);
        }

        [Fact]
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
    }
}
