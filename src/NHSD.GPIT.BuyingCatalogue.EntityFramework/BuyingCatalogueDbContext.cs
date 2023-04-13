using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework
{
    public class BuyingCatalogueDbContext : IdentityDbContext<AspNetUser, AspNetRole, int, AspNetUserClaim, AspNetUserRole, AspNetUserLogin, AspNetRoleClaim, AspNetUserToken>, IDataProtectionKeyContext
    {
        private readonly IIdentityService identityService;

        public BuyingCatalogueDbContext()
        {
        }

        public BuyingCatalogueDbContext(DbContextOptions<BuyingCatalogueDbContext> options)
            : base(options)
        {
        }

        public BuyingCatalogueDbContext(DbContextOptions<BuyingCatalogueDbContext> options, IIdentityService identityService)
            : base(options)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        protected BuyingCatalogueDbContext(
            DbContextOptions options,
            IIdentityService identityService)
            : base(options)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public DbSet<AdditionalService> AdditionalServices { get; set; }

        public DbSet<AssociatedService> AssociatedServices { get; set; }

        public DbSet<Capability> Capabilities { get; set; }

        public DbSet<CapabilityCategory> CapabilityCategories { get; set; }

        public DbSet<Epic> Epics { get; set; }

        public DbSet<CatalogueItem> CatalogueItems { get; set; }

        public DbSet<CatalogueItemCapability> CatalogueItemCapabilities { get; set; }

        public DbSet<CatalogueItemEpic> CatalogueItemEpics { get; set; }

        public DbSet<CataloguePrice> CataloguePrices { get; set; }

        public DbSet<CataloguePriceTier> CataloguePriceTiers { get; set; }

        public DbSet<Framework> Frameworks { get; set; }

        public DbSet<FrameworkCapability> FrameworkCapabilities { get; set; }

        public DbSet<GpPracticeSize> GpPracticeSizes { get; set; }

        public DbSet<MarketingContact> MarketingContacts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<ServiceRecipient> ServiceRecipients { get; set; }

        public DbSet<Solution> Solutions { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<SupplierContact> SupplierContacts { get; set; }

        public DbSet<SupplierServiceAssociation> SupplierServiceAssociations { get; set; }

        public DbSet<Standard> Standards { get; set; }

        public DbSet<StandardCapability> StandardCapabilities { get; set; }

        public DbSet<AspNetUser> AspNetUsers { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public DbSet<Organisation> Organisations { get; set; }

        public DbSet<RelatedOrganisation> RelatedOrganisations { get; set; }

        public DbSet<ServiceLevelAgreements> ServiceLevelAgreements { get; set; }

        public DbSet<SlaContact> SlaContacts { get; set; }

        public DbSet<SlaServiceLevel> SlaServiceLevels { get; set; }

        public DbSet<ServiceAvailabilityTimes> ServiceAvailabilityTimes { get; set; }

        public DbSet<WorkOffPlan> WorkOffPlans { get; set; }

        public DbSet<OrderDeletionApproval> OrderDeletionApprovals { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<OrderItemFunding> OrderItemFunding { get; set; }

        public DbSet<OrderItemRecipient> OrderItemRecipients { get; set; }

        public DbSet<OrderItemPrice> OrderItemPrices { get; set; }

        public DbSet<OrderItemPriceTier> OrderItemPriceTiers { get; set; }

        public DbSet<ContractFlags> ContractFlags { get; set; }

        public DbSet<ImplementationPlan> ImplementationPlans { get; set; }

        public DbSet<ImplementationPlanMilestone> ImplementationPlanMilestones { get; set; }

        public DbSet<ImplementationPlanAcceptanceCriteria> ImplementationPlanAcceptanceCriteria { get; set; }

        public DbSet<EmailDomain> EmailDomains { get; set; }

        public DbSet<OdsOrganisation> OdsOrganisations { get; set; }

        public DbSet<OrganisationRelationship> OrganisationRelationships { get; set; }

        public DbSet<OrganisationRole> OrganisationRoles { get; set; }

        public DbSet<RoleType> OrganisationRoleTypes { get; set; }

        public DbSet<RelationshipType> OrganisationRelationshipTypes { get; set; }

        public DbSet<OrgImportJournal> OrgImportJournal { get; set; }

        public async Task<Order> Order(CallOffId callOffId)
        {
            return await Orders
                .IgnoreQueryFilters()
                .FirstAsync(x => x.OrderNumber == callOffId.OrderNumber && x.Revision == callOffId.Revision);
        }

        public async Task<Order> Order(string internalOrgId, CallOffId callOffId)
        {
            return await Orders.FirstAsync(x => x.OrderNumber == callOffId.OrderNumber
                && x.Revision == callOffId.Revision
                && x.OrderingParty.InternalIdentifier == internalOrgId);
        }

        public async Task<int> OrderId(CallOffId callOffId)
        {
            return (await Order(callOffId)).Id;
        }

        public async Task<int> OrderId(string internalOrgId, CallOffId callOffId)
        {
            return (await Order(internalOrgId, callOffId)).Id;
        }

        public async Task<int> NextOrderNumber()
        {
            var maxOrderNumber = await Orders
                .IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.OrderNumber) ?? 0;

            return maxOrderNumber + 1;
        }

        public async Task<int> NextRevision(int orderNumber)
        {
            var maxRevision = await Orders
                .IgnoreQueryFilters()
                .Where(x => x.OrderNumber == orderNumber)
                .MaxAsync(x => (int?)x.Revision) ?? 0;

            return maxRevision + 1;
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();

            return base.SaveChanges();
        }

        public void SaveChangesAs(int userId)
        {
            UpdateAuditFields(userId);

            base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BuyingCatalogueDbContext).Assembly);
        }

        private void UpdateAuditFields(int? userId = null)
        {
            userId ??= identityService?.GetUserId();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is not IAudited auditedEntity)
                    continue;

                switch (entry.State)
                {
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                        continue;
                    default:
                        auditedEntity.LastUpdatedBy = userId ?? auditedEntity.LastUpdatedBy;
                        auditedEntity.LastUpdated = DateTime.UtcNow;
                        continue;
                }
            }
        }
    }
}
