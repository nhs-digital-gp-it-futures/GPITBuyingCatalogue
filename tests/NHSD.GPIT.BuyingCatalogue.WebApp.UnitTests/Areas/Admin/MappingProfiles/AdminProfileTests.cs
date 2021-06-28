using System;
using AutoMapper;
using Bogus;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
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

        [Fact]
        public void Map_OrganisationToOrganisationModel_ResultAsExpected()
        {
            var organisation = new Faker<Organisation>()
                .RuleFor(o => o.Name, f => f.Company.CompanyName())
                .RuleFor(o => o.OdsCode, f => f.Company.CompanySuffix())
                .RuleFor(o => o.OrganisationId, Guid.NewGuid)
                .Generate();
            
            var actual = mapper.Map<Organisation, OrganisationModel>(organisation);

            actual.Id.Should().Be(organisation.OrganisationId);
            actual.Name.Should().Be(organisation.Name);
            actual.OdsCode.Should().Be(organisation.OdsCode);
        }
    }
}
