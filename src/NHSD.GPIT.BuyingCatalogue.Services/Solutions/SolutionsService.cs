using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EnumsNET;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public sealed class SolutionsService : ISolutionsService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IDbRepository<Solution, BuyingCatalogueDbContext> solutionRepository;
        private readonly IDbRepository<Supplier, BuyingCatalogueDbContext> supplierRepository;
        private readonly ICatalogueItemRepository catalogueItemRepository;

        public SolutionsService(
            BuyingCatalogueDbContext dbContext,
            IDbRepository<Solution, BuyingCatalogueDbContext> solutionRepository,
            IDbRepository<Supplier, BuyingCatalogueDbContext> supplierRepository,
            ICatalogueItemRepository catalogueItemRepository)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.solutionRepository = solutionRepository ?? throw new ArgumentNullException(nameof(solutionRepository));
            this.supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
            this.catalogueItemRepository = catalogueItemRepository ?? throw new ArgumentNullException(nameof(catalogueItemRepository));
        }

        public Task<CatalogueItem> GetSolutionListPrices(CatalogueItemId solutionId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.CataloguePrices).ThenInclude(p => p.PricingUnit)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();
        }

        public Task<CatalogueItem> GetSolution(CatalogueItemId solutionId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemContacts)
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier).ThenInclude(s => s.SupplierContacts)
                .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions).ThenInclude(fs => fs.Framework)
                .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
                .Include(i => i.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Include(i => i.SupplierServiceAssociations)
                .Where(i => i.Id == solutionId)
                .FirstOrDefaultAsync();
        }

        public Task<CatalogueItem> GetSolutionByName(string solutionName)
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
                .Include(i => i.Supplier)
                .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions).ThenInclude(fs => fs.Framework)
                .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
                .Where(i => i.Name == solutionName)
                .FirstOrDefaultAsync();
        }

        public async Task<CatalogueItem> GetSolutionCapability(CatalogueItemId catalogueItemId, int capabilityId)
        {
            return await dbContext.CatalogueItems
                .Include(ci => ci.Solution)
                .Include(ci => ci.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability).ThenInclude(c => c.Epics)
                .Where(c => c.Id == catalogueItemId && c.CatalogueItemCapabilities.Any(sc => sc.CapabilityId == capabilityId))
                .FirstOrDefaultAsync();
        }

        public async Task<CatalogueItem> GetSolutionOverview(CatalogueItemId solutionId)
        {
            var solution = await dbContext.CatalogueItems
                .Include(s => s.CatalogueItemCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier).ThenInclude(s => s.SupplierContacts)
                .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions).ThenInclude(fs => fs.Framework)
                .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
                .Include(s => s.CatalogueItemEpics).ThenInclude(se => se.Status)
                .Include(s => s.CatalogueItemEpics).ThenInclude(se => se.Epic)
                .Include(i => i.CataloguePrices).ThenInclude(p => p.PricingUnit)
                .Where(i => i.Id == solutionId)
                .FirstOrDefaultAsync();

            var associatedServices = await dbContext.CatalogueItems
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService && c.AssociatedService != null).Take(1))
                .ThenInclude(c => c.AssociatedService)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();

            solution?.Supplier?.CatalogueItems.Add(associatedServices.Supplier?.CatalogueItems?.FirstOrDefault());

            var additionalServices = await dbContext.CatalogueItems
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService && c.AdditionalService != null).Take(1))
                .ThenInclude(c => c.AdditionalService)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();

            solution?.Supplier?.CatalogueItems.Add(additionalServices.Supplier?.CatalogueItems?.FirstOrDefault());

            return solution;
        }

        public async Task<CatalogueItem> GetSolutionWithAllAssociatedServices(CatalogueItemId solutionId)
        {
            var solution = await dbContext.CatalogueItems
               .Include(s => s.CatalogueItemCapabilities)
               .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions)
               .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
               .Include(s => s.CatalogueItemEpics)
               .Include(i => i.CataloguePrices)
               .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService && c.PublishedStatus == PublicationStatus.Published)).ThenInclude(c => c.AssociatedService)
               .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService && c.PublishedStatus == PublicationStatus.Published)).ThenInclude(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
               .Where(i => i.Id == solutionId)
               .SingleOrDefaultAsync();

            var additionalServices = await dbContext.CatalogueItems
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService && c.AdditionalService != null).Take(1))
                .ThenInclude(a => a.AdditionalService)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();

            solution?.Supplier?.CatalogueItems.Add(additionalServices.Supplier?.CatalogueItems?.FirstOrDefault());

            return solution;
        }

        public async Task<CatalogueItem> GetSolutionAdditionalServiceCapabilities(CatalogueItemId id)
        {
            var catalogueItem = await dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemCapabilities)
                .ThenInclude(cic => cic.Capability)
                .ThenInclude(c => c.Epics)
                .Where(i => i.Id == id)
                .SingleAsync();

            return catalogueItem;
        }

        public async Task<CatalogueItem> GetAdditionalServiceCapability(
            CatalogueItemId catalogueItemId,
            int capabilityId)
        {
            var catalogueItem = await dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemCapabilities.Where(cic => cic.CapabilityId == capabilityId))
                .ThenInclude(cic => cic.Capability)
                .ThenInclude(c => c.Epics)
                .Where(i => i.Id == catalogueItemId)
                .SingleAsync();

            catalogueItem.CatalogueItemCapabilities ??= Array.Empty<CatalogueItemCapability>();

            return catalogueItem;
        }

        public async Task<CatalogueItem> GetSolutionWithAllAdditionalServices(CatalogueItemId solutionId)
        {
            var solution = await dbContext.CatalogueItems
                .Include(i => i.CatalogueItemCapabilities)
                .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions)
                .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
                .Include(s => s.CatalogueItemEpics)
                .Include(i => i.CataloguePrices)
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService)).ThenInclude(c => c.AdditionalService)
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService)).ThenInclude(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();

            var associatedServices = await dbContext.CatalogueItems
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService && c.AssociatedService != null).Take(1))
                .ThenInclude(a => a.AssociatedService)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();

            solution?.Supplier?.CatalogueItems.Add(associatedServices.Supplier?.CatalogueItems?.FirstOrDefault());

            return solution;
        }

        public async Task SaveSolutionDetails(CatalogueItemId id, string solutionName, int supplierId, IList<FrameworkModel> selectedFrameworks)
        {
            var data = await GetCatalogueItem(id);

            data.Name = solutionName;
            data.SupplierId = supplierId;

            var frameworks = data.Solution.FrameworkSolutions.ToList();
            frameworks.RemoveAll(f => selectedFrameworks.Any(sf => f.FrameworkId == sf.FrameworkId && sf.Selected == false));

            foreach (var framework in selectedFrameworks.Where(x => x.Selected))
            {
                var existingFramework = frameworks.FirstOrDefault(fs => fs.FrameworkId == framework.FrameworkId);

                if (existingFramework is null)
                {
                    frameworks.Add(new FrameworkSolution
                    {
                        FrameworkId = framework.FrameworkId,
                        IsFoundation = framework.IsFoundation,
                        LastUpdated = DateTime.UtcNow,
                    });
                }
                else
                {
                    existingFramework.IsFoundation = framework.IsFoundation;
                    existingFramework.LastUpdated = DateTime.UtcNow;
                }
            }

            data.Solution.FrameworkSolutions = frameworks;
            await dbContext.SaveChangesAsync();
        }

        public async Task SaveSolutionDescription(CatalogueItemId solutionId, string summary, string description, string link)
        {
            summary.ValidateNotNullOrWhiteSpace(nameof(summary));

            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.Summary = summary;
            solution.FullDescription = description;
            solution.AboutUrl = link;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveSolutionFeatures(CatalogueItemId solutionId, string[] features)
        {
            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.Features = JsonSerializer.Serialize(features);
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveImplementationDetail(CatalogueItemId solutionId, string detail)
        {
            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.ImplementationDetail = detail;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveRoadMap(CatalogueItemId solutionId, string roadMap)
        {
            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.RoadMap = roadMap;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task<ClientApplication> GetClientApplication(CatalogueItemId solutionId)
        {
            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            return solution.GetClientApplication();
        }

        public async Task SaveClientApplication(CatalogueItemId solutionId, ClientApplication clientApplication)
        {
            clientApplication.ValidateNotNull(nameof(clientApplication));

            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.ClientApplication = JsonSerializer.Serialize(clientApplication);
            await solutionRepository.SaveChangesAsync();
        }

        public async Task DeleteClientApplication(CatalogueItemId solutionId, ClientApplicationType clientApplicationType)
        {
            var clientApplication = await GetClientApplication(solutionId);

            RemoveClientApplicationType(clientApplication, clientApplicationType);

            await SaveClientApplication(solutionId, clientApplication);
        }

        public async Task<Hosting> GetHosting(CatalogueItemId solutionId)
        {
            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            return solution.Hosting ?? new Hosting();
        }

        public async Task SaveHosting(CatalogueItemId solutionId, Hosting hosting)
        {
            hosting.ValidateNotNull(nameof(hosting));

            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.Hosting = hosting;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveSupplierDescriptionAndLink(int supplierId, string description, string link)
        {
            var supplier = await supplierRepository.SingleAsync(s => s.Id == supplierId);
            supplier.Summary = description;
            supplier.SupplierUrl = link;
            await supplierRepository.SaveChangesAsync();
        }

        public async Task SaveSupplierContacts(SupplierContactsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            model.SetSolutionId();

            var marketingContacts = await dbContext.MarketingContacts.Where(c => c.SolutionId == model.SolutionId).ToListAsync();

            if (!marketingContacts.Any())
            {
                dbContext.MarketingContacts.AddRange(model.ValidContacts());
            }
            else
            {
                foreach (var contact in marketingContacts)
                {
                    if (model.ContactFor(contact.Id) is not { } newContact)
                        continue;

                    if (newContact.IsEmpty())
                        dbContext.MarketingContacts.Remove(contact);
                    else
                        contact.UpdateFrom(newContact);
                }

                dbContext.MarketingContacts.AddRange(model.NewAndValidContacts());
            }

            await dbContext.SaveChangesAsync();
        }

        public Task<List<CatalogueItem>> GetSupplierSolutions(int? supplierId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier)
                .Where(i => i.SupplierId == supplierId.GetValueOrDefault()
                    && i.CatalogueItemType == CatalogueItemType.Solution
                    && i.PublishedStatus == PublicationStatus.Published)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<IList<CatalogueItem>> GetAllSolutions(
            PublicationStatus? publicationStatus = null)
        {
            var query = dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.Supplier)
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution);

            if (publicationStatus is not null)
                query = query.Where(i => i.PublishedStatus == publicationStatus.Value);

            return await query
                .OrderByDescending(i => i.Created)
                .ThenBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<CatalogueItemId> AddCatalogueSolution(CreateSolutionModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            model.Frameworks.ValidateNotNull(nameof(CreateSolutionModel.Frameworks));

            var dateTimeNow = DateTime.UtcNow;

            var frameworkSolutions = new List<FrameworkSolution>();

            foreach (var framework in model.Frameworks.Where(f => f.Selected))
            {
                frameworkSolutions.Add(new FrameworkSolution
                {
                    FrameworkId = framework.FrameworkId,
                    IsFoundation = framework.IsFoundation,
                    LastUpdated = dateTimeNow,
                    LastUpdatedBy = model.UserId,
                });
            }

            var catalogueItem = new CatalogueItem
            {
                CatalogueItemType = CatalogueItemType.Solution,
                Solution =
                        new Solution
                        {
                            FrameworkSolutions = frameworkSolutions,
                            LastUpdated = dateTimeNow,
                            LastUpdatedBy = model.UserId,
                        },
                Name = model.Name,
                PublishedStatus = PublicationStatus.Draft,
                SupplierId = model.SupplierId,
            };

            catalogueItemRepository.Add(catalogueItem);

            await catalogueItemRepository.SaveChangesAsync();

            return catalogueItem.Id;
        }

        public async Task<IList<EntityFramework.Catalogue.Models.Framework>> GetAllFrameworks()
        {
            return await dbContext.Frameworks.ToListAsync();
        }

        public Task<bool> SupplierHasSolutionName(int supplierId, string solutionName) =>
            catalogueItemRepository.SupplierHasSolutionName(supplierId, solutionName);

        public async Task SavePublicationStatus(CatalogueItemId solutionId, PublicationStatus publicationStatus)
        {
            var solution = await GetSolution(solutionId);

            solution.PublishedStatus = publicationStatus;

            await dbContext.SaveChangesAsync();
        }

        public async Task SaveContacts(CatalogueItemId solutionId, IList<SupplierContact> supplierContacts)
        {
            var solution = await GetSolution(solutionId);

            var staleContacts = solution.CatalogueItemContacts.Except(supplierContacts);
            foreach (var staleContact in staleContacts.ToList())
            {
                solution.CatalogueItemContacts.Remove(staleContact);
            }

            solution.CatalogueItemContacts = supplierContacts;

            await dbContext.SaveChangesAsync();
        }

        internal static ClientApplication RemoveClientApplicationType(ClientApplication clientApplication, ClientApplicationType clientApplicationType)
        {
            if (clientApplication is null)
                throw new ArgumentNullException(nameof(clientApplication));

            if (clientApplication.ClientApplicationTypes is not null)
            {
                if (clientApplication.ClientApplicationTypes.Contains(clientApplicationType.AsString(EnumFormat.EnumMemberValue)))
                    clientApplication.ClientApplicationTypes.Remove(clientApplicationType.AsString(EnumFormat.EnumMemberValue));
            }

            if (clientApplicationType == ClientApplicationType.BrowserBased)
            {
                clientApplication.AdditionalInformation = null;
                clientApplication.BrowsersSupported = null;
                clientApplication.HardwareRequirements = null;
                clientApplication.MinimumConnectionSpeed = null;
                clientApplication.MinimumDesktopResolution = null;
                clientApplication.MobileFirstDesign = null;
                clientApplication.MobileResponsive = null;
                clientApplication.Plugins = null;
            }
            else if (clientApplicationType == ClientApplicationType.Desktop)
            {
                clientApplication.NativeDesktopAdditionalInformation = null;
                clientApplication.NativeDesktopHardwareRequirements = null;
                clientApplication.NativeDesktopMemoryAndStorage = null;
                clientApplication.NativeDesktopMinimumConnectionSpeed = null;
                clientApplication.NativeDesktopOperatingSystemsDescription = null;
                clientApplication.NativeDesktopThirdParty = null;
            }
            else
            {
                clientApplication.MobileConnectionDetails = null;
                clientApplication.MobileMemoryAndStorage = null;
                clientApplication.MobileOperatingSystems = null;
                clientApplication.MobileThirdParty = null;
                clientApplication.NativeMobileAdditionalInformation = null;
                clientApplication.NativeMobileFirstDesign = null;
                clientApplication.NativeMobileHardwareRequirements = null;
            }

            return clientApplication;
        }

        private async Task<CatalogueItem> GetCatalogueItem(CatalogueItemId id) => await dbContext.CatalogueItems
            .Include(s => s.Solution)
            .Include(s => s.Solution.FrameworkSolutions)
            .SingleAsync(s => s.Id == id);
    }
}
