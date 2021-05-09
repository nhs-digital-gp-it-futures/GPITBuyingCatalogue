using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.MappingProfiles
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HostingTypeProfileTests
    {
        [Test]
        public static void Mappings_Configuration_Valid()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrganisationProfile>();
                cfg.AddProfile<HostingTypeProfile>();
            });
            
            mapperConfiguration.AssertConfigurationIsValid();
        }
    }
}