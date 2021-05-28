using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
{
    public class ConfigSettingResolver : IMemberValueResolver<object, object, string, string>
    {
        private readonly IConfiguration configuration;

        public ConfigSettingResolver(IConfiguration configuration) => this.configuration = configuration;

        public string Resolve(
            object source,
            object destination,
            string sourceMember,
            string destMember,
            ResolutionContext context) => configuration[sourceMember];
    }
}
