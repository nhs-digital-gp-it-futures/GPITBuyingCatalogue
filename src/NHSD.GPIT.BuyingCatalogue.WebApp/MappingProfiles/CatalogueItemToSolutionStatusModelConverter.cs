using System;
using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
{
    public class CatalogueItemToSolutionStatusModelConverter : ITypeConverter<CatalogueItem, SolutionStatusModel>
    {
        private readonly IMapper _mapper;

        public CatalogueItemToSolutionStatusModelConverter(IMapper mapper)
        {
            _mapper = mapper;
        }

        public SolutionStatusModel Convert(CatalogueItem source, SolutionStatusModel destination, ResolutionContext context)
        {
            if (destination == null)
                throw new ArgumentNullException($"A valid {nameof(SolutionStatusModel)} is required for this call");
            
            destination.AboutSupplierStatus = _mapper.Map<CatalogueItem, AboutSupplierModel>(source)
                .IsComplete.ToStatus();
            destination.BrowserBasedStatus = _mapper.Map<CatalogueItem, BrowserBasedModel>(source)
                .IsComplete.ToStatus();
            destination.ClientApplicationTypeStatus = _mapper.Map<CatalogueItem, ClientApplicationTypesModel>(source)
                .IsComplete.ToStatus();
            destination.ContactDetailsStatus = _mapper.Map<CatalogueItem, ContactDetailsModel>(source)
                .IsComplete.ToStatus();
            destination.FeaturesStatus = _mapper.Map<CatalogueItem, FeaturesModel>(source)
                .IsComplete.ToStatus();
            destination.HybridStatus = _mapper.Map<CatalogueItem, HybridModel>(source)
                .IsComplete.ToStatus();
            destination.ImplementationTimescalesStatus = _mapper.Map<CatalogueItem, ImplementationTimescalesModel>(source)
                .IsComplete.ToStatus();
            destination.IntegrationsStatus = _mapper.Map<CatalogueItem, IntegrationsModel>(source)
                .IsComplete.ToStatus();
            destination.OnPremisesStatus = _mapper.Map<CatalogueItem, OnPremiseModel>(source)
                .IsComplete.ToStatus();
            destination.NativeDesktopStatus = _mapper.Map<CatalogueItem, NativeDesktopModel>(source)
                .IsComplete.ToStatus();
            destination.NativeMobileStatus = _mapper.Map<CatalogueItem, NativeMobileModel>(source)
                .IsComplete.ToStatus();
            destination.PrivateCloudStatus = _mapper.Map<CatalogueItem, PrivateCloudModel>(source)
                .IsComplete.ToStatus();
            destination.PublicCloudStatus = _mapper.Map<CatalogueItem, PublicCloudModel>(source)
                .IsComplete.ToStatus();
            destination.RoadmapStatus = _mapper.Map<CatalogueItem, RoadmapModel>(source)
                .IsComplete.ToStatus();
            destination.SolutionDescriptionStatus = _mapper.Map<CatalogueItem, SolutionDescriptionModel>(source)
                .IsComplete.ToStatus();


            return destination;
        }
    }
}