using System;
using AutoMapper;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
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

        public CatalogueItemToSolutionStatusModelConverter(IMapper mapper) => _mapper = mapper;

        public SolutionStatusModel Convert(CatalogueItem source, SolutionStatusModel destination, ResolutionContext context)
        {
            if (source == null)
                throw new ArgumentNullException($"${nameof(CatalogueItem)} cannot be null");

            var result = new SolutionStatusModel
            {
                BackLink = $"/marketing/supplier/solution/{source.CatalogueItemId}",
                BackLinkText = "Return to all sections",
                CatalogueItemName = source.Name,
                SolutionId = source.CatalogueItemId,
                SupplierId = source.Supplier?.Id,
                SupplierName = source.Supplier?.Name,
            };

            result.AboutSupplierStatus = _mapper.Map<CatalogueItem, AboutSupplierModel>(source)
                    .IsComplete.ToStatus();
            result.BrowserBasedStatus = _mapper.Map<CatalogueItem, BrowserBasedModel>(source)
                .IsComplete.ToStatus();
            result.ClientApplication = source.Solution != null && !string.IsNullOrEmpty(source.Solution.ClientApplication)
                ? JsonConvert.DeserializeObject<ClientApplication>(source.Solution.ClientApplication)
                : new ClientApplication();
            result.ClientApplicationTypeStatus = _mapper.Map<CatalogueItem, ClientApplicationTypesModel>(source)
                    .IsComplete.ToStatus();
            result.ContactDetailsStatus = _mapper.Map<CatalogueItem, ContactDetailsModel>(source)
                    .IsComplete.ToStatus();
            result.FeaturesStatus = _mapper.Map<CatalogueItem, FeaturesModel>(source)
                    .IsComplete.ToStatus();
            result.HybridStatus = _mapper.Map<CatalogueItem, HybridModel>(source)
                    .IsComplete.ToStatus();
            result.ImplementationTimescalesStatus = _mapper.Map<CatalogueItem, ImplementationTimescalesModel>(source)
                    .IsComplete.ToStatus();
            result.IntegrationsStatus = _mapper.Map<CatalogueItem, IntegrationsModel>(source)
                    .IsComplete.ToStatus();
            result.OnPremisesStatus = _mapper.Map<CatalogueItem, OnPremiseModel>(source)
                    .IsComplete.ToStatus();
            result.NativeDesktopStatus = _mapper.Map<CatalogueItem, NativeDesktopModel>(source)
                    .IsComplete.ToStatus();
            result.NativeMobileStatus = _mapper.Map<CatalogueItem, NativeMobileModel>(source)
                    .IsComplete.ToStatus();
            result.PrivateCloudStatus = _mapper.Map<CatalogueItem, PrivateCloudModel>(source)
                    .IsComplete.ToStatus();
            result.PublicCloudStatus = _mapper.Map<CatalogueItem, PublicCloudModel>(source)
                    .IsComplete.ToStatus();
            result.RoadmapStatus = _mapper.Map<CatalogueItem, RoadmapModel>(source)
                    .IsComplete.ToStatus();
            result.SolutionDescriptionStatus = _mapper.Map<CatalogueItem, SolutionDescriptionModel>(source)
                    .IsComplete.ToStatus();
            
            return result;
        }
    }
}