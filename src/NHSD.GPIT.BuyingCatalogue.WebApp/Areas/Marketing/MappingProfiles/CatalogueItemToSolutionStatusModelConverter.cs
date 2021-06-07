using System;
using AutoMapper;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles
{
    public class CatalogueItemToSolutionStatusModelConverter : ITypeConverter<CatalogueItem, SolutionStatusModel>
    {
        private readonly IMapper mapper;

        public CatalogueItemToSolutionStatusModelConverter(IMapper mapper) => this.mapper = mapper;

        public SolutionStatusModel Convert(CatalogueItem catalogueItem, SolutionStatusModel destination, ResolutionContext context)
        {
            if (catalogueItem == null)
                throw new ArgumentNullException(nameof(catalogueItem));

            var result = new SolutionStatusModel
            {
                BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}",
                BackLinkText = "Return to all sections",
                CatalogueItemName = catalogueItem.Name,
                SolutionId = catalogueItem.CatalogueItemId,
                SupplierId = catalogueItem.Supplier?.Id,
                SupplierName = catalogueItem.Supplier?.Name,
            };

            result.AboutSupplierStatus = mapper.Map<CatalogueItem, AboutSupplierModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.BrowserBasedStatus = mapper.Map<CatalogueItem, BrowserBasedModel>(catalogueItem)
                .IsComplete.ToStatus();
            result.ClientApplication = catalogueItem.Solution != null && !string.IsNullOrEmpty(catalogueItem.Solution.ClientApplication)
                ? JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication)
                : new ClientApplication();
            result.ClientApplicationTypeStatus = mapper.Map<CatalogueItem, ClientApplicationTypesModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.ContactDetailsStatus = mapper.Map<CatalogueItem, ContactDetailsModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.FeaturesStatus = mapper.Map<CatalogueItem, FeaturesModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.HybridStatus = mapper.Map<CatalogueItem, HybridModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.ImplementationTimescalesStatus = mapper.Map<CatalogueItem, ImplementationTimescalesModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.IntegrationsStatus = mapper.Map<CatalogueItem, IntegrationsModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.OnPremisesStatus = mapper.Map<CatalogueItem, OnPremiseModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.NativeDesktopStatus = mapper.Map<CatalogueItem, NativeDesktopModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.NativeMobileStatus = mapper.Map<CatalogueItem, NativeMobileModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.PrivateCloudStatus = mapper.Map<CatalogueItem, PrivateCloudModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.PublicCloudStatus = mapper.Map<CatalogueItem, PublicCloudModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.RoadmapStatus = mapper.Map<CatalogueItem, RoadmapModel>(catalogueItem)
                    .IsComplete.ToStatus();
            result.SolutionDescriptionStatus = mapper.Map<CatalogueItem, SolutionDescriptionModel>(catalogueItem)
                    .IsComplete.ToStatus();

            return result;
        }
    }
}
