using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles
{
    public class IntegrationModelsResolver : IMemberValueResolver<CatalogueItem, InteroperabilityModel, string,
        IList<IntegrationModel>>
    {
        private readonly IMapper mapper;

        public IntegrationModelsResolver(IMapper mapper) => this.mapper = mapper;

        public IList<IntegrationModel> Resolve(
            CatalogueItem source,
            InteroperabilityModel destination,
            string sourceMember,
            IList<IntegrationModel> destMember,
            ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(sourceMember))
                return new List<IntegrationModel>();

            var integrations = JsonConvert.DeserializeObject<List<Integration>>(sourceMember);

            if (integrations == null || !integrations.Any())
                return new List<IntegrationModel>();

            return mapper.Map<IList<Integration>, IList<IntegrationModel>>(integrations);
        }
    }
}
