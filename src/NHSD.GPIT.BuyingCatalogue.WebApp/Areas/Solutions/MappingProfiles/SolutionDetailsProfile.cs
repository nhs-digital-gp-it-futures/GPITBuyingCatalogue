using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.MappingProfiles
{
    public class SolutionDetailsProfile : Profile
    {
        private const string KeyBrowserBased = "browser-based";
        private const string KeyNativeDesktop = "native-desktop";
        private const string KeyNativeMobile = "native-mobile";

        public SolutionDetailsProfile()
        {
            CreateMap<CatalogueItem, ClientApplicationTypesModel>()
                .ForMember(
                    dest => dest.ApplicationTypes,
                    opt => opt.MapFrom(
                        (_, dest, _) => new DescriptionListViewModel
                        {
                            Heading = "Type of application",
                            Items = new Dictionary<string, ListViewModel>
                            {
                                {
                                    "Browser-based application",
                                    new ListViewModel { Text = dest.HasApplicationType(KeyBrowserBased) }
                                },
                                {
                                    "Desktop application",
                                    new ListViewModel { Text = dest.HasApplicationType(KeyNativeDesktop) }
                                },
                                {
                                    "Mobile application",
                                    new ListViewModel { Text = dest.HasApplicationType(KeyNativeMobile) }
                                },
                            },
                        }))
                .ForMember(dest => dest.BrowserBasedApplication, opt => opt.Ignore())
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Client application type"))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>()
                .AfterMap(
                    (_, dest) =>
                    {
                        dest.BrowserBasedApplication = new DescriptionListViewModel
                        {
                            Heading = "Browser-based application",
                        };
                        if (dest.ClientApplication == null)
                            return;

                        if (dest.ClientApplication.BrowsersSupported?.Any() == true)
                        {
                            dest.BrowserBasedApplication.Items.Add(
                                "Supported browser types",
                                new ListViewModel { List = dest.ClientApplication.BrowsersSupported.ToArray(), });
                        }

                        dest.BrowserBasedApplication.Items.Add(
                            "Mobile responsive",
                            new ListViewModel { Text = dest.ClientApplication.MobileResponsive.ToYesNo(), });

                        dest.BrowserBasedApplication.Items.Add(
                            "Plug-ins or extensions required",
                            new ListViewModel { Text = dest.ClientApplication.Plugins.Required.ToYesNo(), });

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.MinimumConnectionSpeed))
                        {
                            dest.BrowserBasedApplication.Items.Add(
                                "Minimum connection speed",
                                new ListViewModel { Text = dest.ClientApplication.MinimumConnectionSpeed, });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.MinimumDesktopResolution))
                        {
                            dest.BrowserBasedApplication.Items.Add(
                                "Screen resolution and aspect ratio",
                                new ListViewModel { Text = dest.ClientApplication.MinimumDesktopResolution, });
                        }

                        if (!string.IsNullOrWhiteSpace(dest.ClientApplication.HardwareRequirements))
                        {
                            dest.BrowserBasedApplication.Items.Add(
                                "Hardware requirements",
                                new ListViewModel { Text = dest.ClientApplication.HardwareRequirements });
                        }
                    });

            CreateMap<CatalogueItem, SolutionDisplayBaseModel>()
                .BeforeMap(
                    (src, dest) => dest.SetClientApplication(
                        src.Solution != null && !string.IsNullOrEmpty(src.Solution.ClientApplication)
                            ? JsonConvert.DeserializeObject<ClientApplication>(src.Solution.ClientApplication)
                            : new ClientApplication()))
                .ForMember(dest => dest.ClientApplication, opt => opt.Ignore())
                .ForMember(
                    dest => dest.LastReviewed,
                    opt => opt.MapFrom<IMemberValueResolver<object, object, string, string>, string>(
                        x => "SolutionsLastReviewedDate"))
                .ForMember(dest => dest.PaginationFooter, opt => opt.Ignore())
                .ForMember(dest => dest.Section, opt => opt.Ignore())
                .ForMember(dest => dest.SolutionId, opt => opt.MapFrom(src => src.CatalogueItemId))
                .ForMember(dest => dest.SolutionName, opt => opt.MapFrom(src => src.Name))
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .AfterMap(
                    (_, dest) =>
                    {
                        (SectionModel previous, SectionModel next) = dest.PreviousAndNextModels();
                        dest.PaginationFooter = new PaginationFooterModel { Next = next, Previous = previous, };
                    });

            CreateMap<CatalogueItem, ImplementationTimescalesModel>()
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.ImplementationDetail))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Implementation timescales"))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>();

            CreateMap<CatalogueItem, SolutionDescriptionModel>()
                .ForMember(
                    dest => dest.Description,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.FullDescription))
                .ForMember(dest => dest.Framework, opt => opt.MapFrom(src => src.Framework()))
                .ForMember(dest => dest.IsFoundation, opt => opt.MapFrom(src => src.IsFoundation().ToYesNo()))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Description"))
                .ForMember(
                    dest => dest.Summary,
                    opt => opt.MapFrom(src => src.Solution == null ? null : src.Solution.Summary))
                .ForMember(
                    dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier == null ? null : src.Supplier.Name))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>();

            CreateMap<CatalogueItem, SolutionFeaturesModel>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features()))
                .ForMember(dest => dest.Section, opt => opt.MapFrom(src => "Features"))
                .IncludeBase<CatalogueItem, SolutionDisplayBaseModel>()
                .AfterMap(
                    (_, dest) =>
                    {
                        dest.PaginationFooter.Previous = dest.GetSectionFor("Description");
                        dest.PaginationFooter.Next = dest.GetSectionFor("Capabilities");
                    });
        }
    }
}
