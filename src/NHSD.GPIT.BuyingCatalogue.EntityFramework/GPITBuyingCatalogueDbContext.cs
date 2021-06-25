using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework
{
    public partial class GPITBuyingCatalogueDbContext : IdentityDbContext<AspNetUser, AspNetRole, string, AspNetUserClaim, AspNetUserRole, AspNetUserLogin, AspNetRoleClaim, AspNetUserToken>
    {
        public GPITBuyingCatalogueDbContext()
        {
        }

        public GPITBuyingCatalogueDbContext(DbContextOptions<GPITBuyingCatalogueDbContext> options)
            : base(options)
        {
        }

        protected GPITBuyingCatalogueDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<AdditionalService> AdditionalServices { get; set; }

        public virtual DbSet<AssociatedService> AssociatedServices { get; set; }

        public virtual DbSet<Capability> Capabilities { get; set; }

        public virtual DbSet<CapabilityCategory> CapabilityCategories { get; set; }

        public virtual DbSet<CapabilityStatus> CapabilityStatuses { get; set; }

        public virtual DbSet<CatalogueItem> CatalogueItems { get; set; }

        public virtual DbSet<CataloguePrice> CataloguePrices { get; set; }

        public virtual DbSet<CataloguePriceTier> CataloguePriceTiers { get; set; }

        public virtual DbSet<CompliancyLevel> CompliancyLevels { get; set; }

        public DbSet<DefaultDeliveryDate> DefaultDeliveryDates { get; set; }

        public virtual DbSet<Epic> Epics { get; set; }

        public virtual DbSet<Framework> Frameworks { get; set; }

        public virtual DbSet<FrameworkCapability> FrameworkCapabilities { get; set; }

        public virtual DbSet<FrameworkSolution> FrameworkSolutions { get; set; }

        public virtual DbSet<MarketingContact> MarketingContacts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public virtual DbSet<PricingUnit> PricingUnits { get; set; }

        public DbSet<ServiceRecipient> ServiceRecipients { get; set; }

        public virtual DbSet<Solution> Solutions { get; set; }

        public virtual DbSet<SolutionCapability> SolutionCapabilities { get; set; }

        public virtual DbSet<SolutionCapabilityStatus> SolutionCapabilityStatuses { get; set; }

        public virtual DbSet<SolutionEpic> SolutionEpics { get; set; }

        public virtual DbSet<SolutionEpicStatus> SolutionEpicStatuses { get; set; }

        public virtual DbSet<Supplier> Suppliers { get; set; }

        public virtual DbSet<SupplierContact> SupplierContacts { get; set; }

        public virtual DbSet<SupplierServiceAssociation> SupplierServiceAssociations { get; set; }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }

        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

        public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public virtual DbSet<Organisation> Organisations { get; set; }

        public virtual DbSet<RelatedOrganisation> RelatedOrganisations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AdditionalService>(entity =>
            {
                entity.HasKey(e => e.CatalogueItemId);
                entity.ToTable("AdditionalService");

                entity.Property(e => e.CatalogueItemId)
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.Property(e => e.FullDescription).HasMaxLength(3000);

                entity.Property(e => e.SolutionId)
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.Property(e => e.Summary).HasMaxLength(300);

                entity.HasOne(d => d.CatalogueItem)
                    .WithOne(p => p.AdditionalService)
                    .HasForeignKey<AdditionalService>(d => d.CatalogueItemId)
                    .HasConstraintName("FK_AdditionalService_CatalogueItem");

                entity.HasOne(d => d.Solution)
                    .WithMany(p => p.AdditionalServices)
                    .HasForeignKey(d => d.SolutionId)
                    .HasConstraintName("FK_AdditionalService_Solution");
            });

            modelBuilder.Entity<AssociatedService>(entity =>
            {
                entity.ToTable("AssociatedService");

                entity.Property(e => e.AssociatedServiceId)
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.OrderGuidance).HasMaxLength(1000);

                entity.HasOne(d => d.AssociatedServiceNavigation)
                    .WithOne(p => p.AssociatedService)
                    .HasForeignKey<AssociatedService>(d => d.AssociatedServiceId)
                    .HasConstraintName("FK_SupplierService_CatalogueItem");
            });

            modelBuilder.Entity<Capability>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);
                entity.ToTable("Capability");
                entity.HasIndex(e => e.CapabilityRef, "IX_CapabilityCapabilityRef")
                    .IsClustered();
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.CapabilityRef)
                    .IsRequired()
                    .HasMaxLength(10);
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.Property(e => e.EffectiveDate).HasColumnType("date");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.PreviousVersion).HasMaxLength(10);
                entity.Property(e => e.SourceUrl).HasMaxLength(1000);
                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Capabilities)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Capability_CapabilityCategory");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Capabilities)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Capability_CapabilityStatus");
            });

            modelBuilder.Entity<CapabilityCategory>(entity =>
            {
                entity.ToTable("CapabilityCategory");
                entity.HasIndex(e => e.Name, "IX_CapabilityCategoryName")
                    .IsUnique();
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CapabilityStatus>(entity =>
            {
                entity.ToTable("CapabilityStatus");
                entity.HasIndex(e => e.Name, "IX_CapabilityStatusName")
                    .IsUnique();
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(16);
            });

            modelBuilder.Entity<CatalogueItem>(entity =>
            {
                entity.ToTable("CatalogueItem");

                entity.Property(e => e.CatalogueItemId)
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.Property(e => e.CatalogueItemType)
                    .HasConversion<int>()
                    .HasColumnName("CatalogueItemTypeId");
                entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.PublishedStatus)
                    .HasConversion<int>()
                    .HasColumnName("PublishedStatusId")
                    .HasDefaultValue(PublicationStatus.Draft);
                entity.Property(e => e.SupplierId)
                    .IsRequired()
                    .HasMaxLength(6);
                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.CatalogueItems)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CatalogueItem_Supplier");
            });

            modelBuilder.Entity<CataloguePrice>(entity =>
            {
                entity.ToTable("CataloguePrice");

                entity.Property(e => e.CatalogueItemId)
                    .IsRequired()
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.Property(e => e.CataloguePriceType)
                    .HasConversion<int>()
                    .HasColumnName("CataloguePriceTypeId");
                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasMaxLength(3);
                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
                entity.Property(e => e.ProvisioningType)
                    .HasConversion<int>()
                    .HasColumnName("ProvisioningTypeId");
                entity.Property(e => e.TimeUnit)
                    .HasConversion<int>()
                    .HasColumnName("TimeUnitId");

                entity.HasOne(d => d.CatalogueItem)
                    .WithMany(p => p.CataloguePrices)
                    .HasForeignKey(d => d.CatalogueItemId);

                entity.HasOne(d => d.PricingUnit)
                    .WithMany(p => p.CataloguePrices)
                    .HasForeignKey(d => d.PricingUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CataloguePriceTier>(entity =>
            {
                entity.ToTable("CataloguePriceTier");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 3)");

                entity.HasOne(d => d.CataloguePrice)
                    .WithMany(p => p.CataloguePriceTiers)
                    .HasForeignKey(d => d.CataloguePriceId);
            });

            modelBuilder.Entity<CompliancyLevel>(entity =>
            {
                entity.ToTable("CompliancyLevel");
                entity.HasIndex(e => e.Name, "IX_CompliancyLevelName")
                    .IsUnique();
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(16);
            });

            modelBuilder.Entity<Epic>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);
                entity.ToTable("Epic");
                entity.Property(e => e.Id).HasMaxLength(10);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(e => e.Active).IsRequired();
                entity.Property(e => e.SupplierDefined).IsRequired().HasDefaultValue(false);
                entity.HasOne(d => d.Capability)
                    .WithMany(p => p.Epics)
                    .HasForeignKey(d => d.CapabilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Epic_Capability");
                entity.HasOne(d => d.CompliancyLevel)
                    .WithMany(p => p.Epics)
                    .HasForeignKey(d => d.CompliancyLevelId)
                    .HasConstraintName("FK_Epic_CompliancyLevel");
            });

            modelBuilder.Entity<Framework>(entity =>
            {
                entity.ToTable("Framework");
                entity.Property(e => e.Id).HasMaxLength(10);
                entity.Property(e => e.ActiveDate).HasColumnType("date");
                entity.Property(e => e.ExpiryDate).HasColumnType("date");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Owner).HasMaxLength(100);
                entity.Property(e => e.ShortName).HasMaxLength(25);
            });

            modelBuilder.Entity<FrameworkCapability>(entity =>
            {
                entity.HasKey(e => new { e.FrameworkId, e.CapabilityId });
                entity.Property(e => e.FrameworkId).HasMaxLength(10);
                entity.HasOne(d => d.Capability)
                    .WithMany(p => p.FrameworkCapabilities)
                    .HasForeignKey(d => d.CapabilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FrameworkCapabilities_Capability");

                entity.HasOne(d => d.Framework)
                    .WithMany(p => p.FrameworkCapabilities)
                    .HasForeignKey(d => d.FrameworkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FrameworkCapabilities_Framework");
            });

            modelBuilder.Entity<FrameworkSolution>(entity =>
            {
                entity.HasKey(e => new { e.FrameworkId, e.SolutionId });
                entity.Property(e => e.FrameworkId).HasMaxLength(10);

                entity.Property(e => e.SolutionId)
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.HasOne(d => d.Framework)
                    .WithMany(p => p.FrameworkSolutions)
                    .HasForeignKey(d => d.FrameworkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FrameworkSolutions_Framework");

                entity.HasOne(d => d.Solution)
                    .WithMany(p => p.FrameworkSolutions)
                    .HasForeignKey(d => d.SolutionId)
                    .HasConstraintName("FK_FrameworkSolutions_Solution");
            });

            modelBuilder.Entity<MarketingContact>(entity =>
            {
                entity.HasKey(e => new { e.SolutionId, e.Id });
                entity.ToTable("MarketingContact");

                entity.Property(e => e.SolutionId)
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Department).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.FirstName).HasMaxLength(35);
                entity.Property(e => e.LastName).HasMaxLength(35);
                entity.Property(e => e.PhoneNumber).HasMaxLength(35);
                entity.HasOne(d => d.Solution)
                    .WithMany(p => p.MarketingContacts)
                    .HasForeignKey(d => d.SolutionId)
                    .HasConstraintName("FK_MarketingContact_Solution");
            });

            modelBuilder.Entity<PricingUnit>(entity =>
            {
                entity.HasKey(e => e.PricingUnitId)
                    .IsClustered(false);
                entity.ToTable("PricingUnit");
                entity.Property(e => e.PricingUnitId).ValueGeneratedNever();
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.TierName)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Solution>(entity =>
            {
                entity.ToTable("Solution");

                entity.Property(e => e.Id)
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.Property(e => e.AboutUrl).HasMaxLength(1000);
                entity.Property(e => e.FullDescription).HasMaxLength(3000);
                entity.Property(e => e.ImplementationDetail).HasMaxLength(1100);
                entity.Property(e => e.IntegrationsUrl).HasMaxLength(1000);
                entity.Property(e => e.RoadMap).HasMaxLength(1000);
                entity.Property(e => e.ServiceLevelAgreement).HasMaxLength(1000);
                entity.Property(e => e.Summary).HasMaxLength(350);
                entity.Property(e => e.Version).HasMaxLength(10);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Solution)
                    .HasForeignKey<Solution>(d => d.Id)
                    .HasConstraintName("FK_Solution_CatalogueItem");
            });

            modelBuilder.Entity<SolutionCapability>(entity =>
            {
                entity.HasKey(e => new { e.SolutionId, e.CapabilityId });
                entity.ToTable("SolutionCapability");

                entity.Property(e => e.SolutionId)
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.HasOne(d => d.Capability)
                    .WithMany(p => p.SolutionCapabilities)
                    .HasForeignKey(d => d.CapabilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SolutionCapability_Capability");
                entity.HasOne(d => d.Solution)
                    .WithMany(p => p.SolutionCapabilities)
                    .HasForeignKey(d => d.SolutionId)
                    .HasConstraintName("FK_SolutionCapability_Solution");
                entity.HasOne(d => d.Status)
                    .WithMany(p => p.SolutionCapabilities)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SolutionCapability_SolutionCapabilityStatus");
            });

            modelBuilder.Entity<SolutionCapabilityStatus>(entity =>
            {
                entity.ToTable("SolutionCapabilityStatus");
                entity.HasIndex(e => e.Name, "IX_SolutionCapabilityStatusName")
                    .IsUnique();
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(16);
            });

            modelBuilder.Entity<SolutionEpic>(entity =>
            {
                entity.HasKey(e => new { e.SolutionId, e.CapabilityId, e.EpicId });
                entity.ToTable("SolutionEpic");

                entity.Property(e => e.SolutionId)
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.Property(e => e.EpicId).HasMaxLength(10);

                entity.HasOne(d => d.Capability)
                    .WithMany(p => p.SolutionEpics)
                    .HasForeignKey(d => d.CapabilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SolutionEpic_Capability");

                entity.HasOne(d => d.Epic)
                    .WithMany(p => p.SolutionEpics)
                    .HasForeignKey(d => d.EpicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SolutionEpic_Epic");

                entity.HasOne(d => d.Solution)
                    .WithMany(p => p.SolutionEpics)
                    .HasForeignKey(d => d.SolutionId)
                    .HasConstraintName("FK_SolutionEpic_Solution");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.SolutionEpics)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SolutionEpicStatus");
            });

            modelBuilder.Entity<SolutionEpicStatus>(entity =>
            {
                entity.ToTable("SolutionEpicStatus");
                entity.HasIndex(e => e.Name, "IX_EpicStatusName")
                    .IsUnique();
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(16);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");
                entity.HasIndex(e => e.Name, "IX_SupplierName");
                entity.Property(e => e.Id).HasMaxLength(6);
                entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasConversion(
                    a => JsonConvert.SerializeObject(a),
                    a => JsonConvert.DeserializeObject<Address>(a));
                entity.Property(e => e.LegalName)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.OdsCode).HasMaxLength(8);
                entity.Property(e => e.Summary).HasMaxLength(1100);
                entity.Property(e => e.SupplierUrl).HasMaxLength(1000);
            });

            modelBuilder.Entity<SupplierContact>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);
                entity.ToTable("SupplierContact");
                entity.HasIndex(e => e.SupplierId, "IX_SupplierContactSupplierId")
                    .IsClustered();
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(35);
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(35);
                entity.Property(e => e.PhoneNumber).HasMaxLength(35);
                entity.Property(e => e.SupplierId)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.SupplierContacts)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_SupplierContact_Supplier");
            });

            modelBuilder.Entity<SupplierServiceAssociation>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("SupplierServiceAssociation");
                entity.Property(e => e.AssociatedServiceId)
                    .IsRequired()
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.Property(e => e.CatalogueItemId)
                    .IsRequired()
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.HasOne(d => d.AssociatedService)
                    .WithMany()
                    .HasForeignKey(d => d.AssociatedServiceId)
                    .HasConstraintName("FK_SupplierServiceAssociation_AssociatedService");

                entity.HasOne(d => d.CatalogueItem)
                    .WithMany()
                    .HasForeignKey(d => d.CatalogueItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupplierServiceAssociation_CatalogueItem");
            });

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

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
