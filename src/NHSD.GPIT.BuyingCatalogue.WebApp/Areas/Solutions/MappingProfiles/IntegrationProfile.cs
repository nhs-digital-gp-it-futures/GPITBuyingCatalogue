using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles
{
    public class IntegrationProfile : Profile
    {
        public IntegrationProfile()
        {
            CreateMap<CatalogueItem, InteroperabilityModel>()
                .ForMember(
                    dest => dest.Integrations,
                    opt =>
                    {
                        opt.PreCondition(c => !string.IsNullOrWhiteSpace(c.Solution?.Integrations));
                        opt.MapFrom<IMemberValueResolver<CatalogueItem, InteroperabilityModel, string,
                            IList<IntegrationModel>>, string>(c => c.Solution.Integrations);
                    })
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>()
                .AfterMap((_, dest) => dest.PaginationFooter.FullWidth = true);

            CreateMap<Integration, IntegrationModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Tables, opt => opt.MapFrom(src => src.SubTypes))
                .IgnoreAllPropertiesWithAnInaccessibleSetter();

            CreateMap<IntegrationSubType, IntegrationTableModel>()
                .ForMember(dest => dest.Headings, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Rows, opt => opt.Ignore())
                .AfterMap(
                    (src, dest) =>
                    {
                        var details = src.DetailsDictionary ?? new Dictionary<string, string>();
                        if (details.Any())
                        {
                            dest.Headings = new List<string>
                            {
                                "Provider or consumer", "Additional information",
                            };
                            foreach (var (role, description) in details)
                            {
                                dest.Rows.Add(new[] { role, description });
                            }

                            return;
                        }

                        var detailsSystem = src.DetailsSystemDictionary
                            ?? new Dictionary<string, Dictionary<string, string>>();

                        dest.Headings = new List<string>
                        {
                            "Provider or consumer", "System integrating with", "Description",
                        };

                        foreach (var (role, dictionary) in detailsSystem)
                        {
                            foreach (var (system, description) in dictionary)
                            {
                                dest.Rows.Add(new[] { role, system, description });
                            }
                        }
                    });
        }
    }
}
