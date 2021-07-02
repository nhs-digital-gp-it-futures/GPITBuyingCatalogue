using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework
{
    public partial class GPITBuyingCatalogueDbContext : IdentityDbContext<AspNetUser, AspNetRole, string, AspNetUserClaim, AspNetUserRole, AspNetUserLogin, AspNetRoleClaim, AspNetUserToken>
    {
        private readonly IIdentityService identityService;

        public GPITBuyingCatalogueDbContext()
        {
        }

        public GPITBuyingCatalogueDbContext(DbContextOptions<GPITBuyingCatalogueDbContext> options, IIdentityService identityService)
            : base(options)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        protected GPITBuyingCatalogueDbContext(
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

        public DbSet<CataloguePrice> CataloguePrices { get; set; }

        public DbSet<DefaultDeliveryDate> DefaultDeliveryDates { get; set; }

        public DbSet<FrameworkCapability> FrameworkCapabilities { get; set; }

        public DbSet<MarketingContact> MarketingContacts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<ServiceRecipient> ServiceRecipients { get; set; }

        public DbSet<Solution> Solutions { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<AspNetUser> AspNetUsers { get; set; }

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

            ApplyCatalogueConfiguration(modelBuilder);

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.OrganisationFunction)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(35);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Organisation>(entity =>
            {
                entity.HasKey(e => e.OrganisationId)
                    .IsClustered(false);

                entity.HasIndex(e => e.Name, "IX_OrganisationName")
                    .IsClustered();

                entity.Property(e => e.OrganisationId).ValueGeneratedNever();

                entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.OdsCode).HasMaxLength(8);

                entity.Property(e => e.PrimaryRoleId).HasMaxLength(8);
            });

            modelBuilder.Entity<RelatedOrganisation>(entity =>
            {
                entity.HasKey(e => new { e.OrganisationId, e.RelatedOrganisationId });

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.RelatedOrganisationOrganisations)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RelatedOrganisations_OrganisationId");

                entity.HasOne(d => d.RelatedOrganisationNavigation)
                    .WithMany(p => p.RelatedOrganisationRelatedOrganisationNavigations)
                    .HasForeignKey(d => d.RelatedOrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RelatedOrganisations_RelatedOrganisationId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        private static void ApplyCatalogueConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AdditionalServiceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AssociatedServiceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CapabilityEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CapabilityCategoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CatalogueItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CataloguePriceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CataloguePriceTierEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EpicEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FrameworkEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FrameworkCapabilityEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FrameworkSolutionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MarketingContactEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PricingUnitEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SolutionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SolutionCapabilityEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SolutionCapabilityStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SolutionEpicEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SolutionEpicStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SupplierEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SupplierContactEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SupplierServiceAssociationEntityTypeConfiguration());
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
