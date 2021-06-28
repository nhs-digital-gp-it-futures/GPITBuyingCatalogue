using System;
using AutoMapper;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.MappingProfiles
{
    public sealed class AdminProfileTests : IDisposable
    {
        private IMapper mapper;
        private MapperConfiguration mapperConfiguration;

        public AdminProfileTests()
        {
            mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AdminProfile>();
            });
            mapper = mapperConfiguration.CreateMapper();
        }

        public void Dispose()
        {
            mapperConfiguration = null;
            mapper = null;
        }

        [Fact]
        public void Mappings_Configuration_Valid()
        {
            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Theory]
        [CommonAutoData]
        public void Map_OrganisationToOrganisationModel_ResultAsExpected(Organisation organisation)
        {
            var actual = mapper.Map<Organisation, OrganisationModel>(organisation);

            actual.Id.Should().Be(organisation.OrganisationId);
            actual.Name.Should().Be(organisation.Name);
            actual.OdsCode.Should().Be(organisation.OdsCode);
        }
    }
}
