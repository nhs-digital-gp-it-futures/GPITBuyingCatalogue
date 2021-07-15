using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework
{
    public class BuyingCatalogueDbContext : IdentityDbContext<AspNetUser, AspNetRole, string, AspNetUserClaim, AspNetUserRole, AspNetUserLogin, AspNetRoleClaim, AspNetUserToken>, IDataProtectionKeyContext
    {
        private readonly IIdentityService identityService;

        public BuyingCatalogueDbContext()
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

        public DbSet<CatalogueItem> CatalogueItems { get; set; }

        public DbSet<CatalogueItemCapability> CatalogueItemCapabilities { get; set; }

        public DbSet<CatalogueItemEpic> CatalogueItemEpics { get; set; }

        public DbSet<CataloguePrice> CataloguePrices { get; set; }

        public DbSet<DefaultDeliveryDate> DefaultDeliveryDates { get; set; }

        public DbSet<FrameworkCapability> FrameworkCapabilities { get; set; }

        public DbSet<Framework> Frameworks { get; set; }

        public DbSet<MarketingContact> MarketingContacts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<ServiceRecipient> ServiceRecipients { get; set; }

        public DbSet<Solution> Solutions { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<AspNetUser> AspNetUsers { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public DbSet<Organisation> Organisations { get; set; }

        public DbSet<RelatedOrganisation> RelatedOrganisations { get; set; }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                // TODO: Don't like this option of not having an identityService but a lot of tests will require fixing up
                if (entry.Entity is not IAudited auditedEntity || identityService is null)
                    continue;

                (Guid userId, string userName) = identityService.GetUserInfo();

                switch (entry.State)
                {
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                        continue;
                    default:
                        auditedEntity.SetLastUpdatedBy(userId, userName);
                        continue;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
