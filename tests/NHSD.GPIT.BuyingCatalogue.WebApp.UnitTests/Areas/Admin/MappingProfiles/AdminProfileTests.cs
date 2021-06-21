using AutoMapper;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.MappingProfiles
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class AdminProfileTests
    {
        private IMapper mapper;
        private MapperConfiguration mapperConfiguration;

        [OneTimeSetUp]
        public void SetUp()
        {
            mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AdminProfile>();
            });
            mapper = mapperConfiguration.CreateMapper();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            mapperConfiguration = null;
            mapper = null;
        }

        [Test]
        public void Mappings_Configuration_Valid()
        {
            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Test, CommonAutoData]
        public void Map_OrganisationToOrganisationModel_ResultAsExpected(Organisation organisation)
        {
            var actual = mapper.Map<Organisation, OrganisationModel>(organisation);

            actual.Id.Should().Be(organisation.OrganisationId);
            actual.Name.Should().Be(organisation.Name);
            actual.OdsCode.Should().Be(organisation.OdsCode);
        }
    }
}
