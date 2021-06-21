using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework
{
    public partial class OrderingDbContext : DbContext
    {
        public OrderingDbContext()
        {
        }

        public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; }

        public virtual DbSet<CatalogueItem> CatalogueItems { get; set; }

        public virtual DbSet<Contact> Contacts { get; set; }

        public virtual DbSet<DefaultDeliveryDate> DefaultDeliveryDates { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<OrderItemRecipient> OrderItemRecipients { get; set; }

        public virtual DbSet<OrderProgress> OrderProgresses { get; set; }

        public virtual DbSet<OrderingParty> OrderingParties { get; set; }

        public virtual DbSet<PricingUnit> PricingUnits { get; set; }

        public virtual DbSet<SelectedServiceRecipient> SelectedServiceRecipients { get; set; }

        public virtual DbSet<ServiceInstanceItem> ServiceInstanceItems { get; set; }

        public virtual DbSet<ServiceRecipient> ServiceRecipients { get; set; }

        public virtual DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");
                entity.Property(e => e.Country).HasMaxLength(256);
                entity.Property(e => e.County).HasMaxLength(256);
                entity.Property(e => e.Line1)
                    .IsRequired()
                    .HasMaxLength(256);
                entity.Property(e => e.Line2).HasMaxLength(256);
                entity.Property(e => e.Line3).HasMaxLength(256);
                entity.Property(e => e.Line4).HasMaxLength(256);
                entity.Property(e => e.Line5).HasMaxLength(256);
                entity.Property(e => e.Postcode)
                    .IsRequired()
                    .HasMaxLength(10);
                entity.Property(e => e.Town).HasMaxLength(256);
            });

            modelBuilder.Entity<CatalogueItem>(entity =>
            {
                entity.ToTable("CatalogueItem");
                entity.Property(e => e.Id)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
                entity.Property(e => e.ParentCatalogueItemId).HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.HasOne(d => d.ParentCatalogueItem)
                    .WithMany(p => p.InverseParentCatalogueItem)
                    .HasForeignKey(d => d.ParentCatalogueItemId)
                    .HasConstraintName("FK_CatalogueItem_ParentCatalogueItem");

                entity.Property(e => e.CatalogueItemType)
                    .HasColumnName("CatalogueItemTypeId")
                    .HasConversion(e => e.Id, e => Models.EnumerationBase.FromId<CatalogueItemType>(e));
            });

            modelBuilder.Entity<CataloguePriceType>(entity =>
            {
                entity.ToTable("CataloguePriceType");
                entity.HasIndex(e => e.Name, "AK_CataloguePriceType_Name")
                    .IsUnique();
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(35);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");
                entity.HasIndex(e => e.IsDeleted, "IX_Order_IsDeleted");
                entity.Property(e => e.CallOffId)
                    .IsRequired()
                    .HasMaxLength(4000)
                    .HasComputedColumnSql("(concat('C',format([Id],'000000'),'-',format([Revision],'00')))", false)
                    .HasConversion(id => id.ToString(), id => CallOffId.Parse(id));

                entity.Property(e => e.CommencementDate).HasColumnType("date");
                entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.FundingSourceOnlyGms).HasColumnName("FundingSourceOnlyGMS");
                entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.LastUpdatedBy);
                entity.Property(e => e.LastUpdatedByName).HasMaxLength(256);
                entity.Property(e => e.Revision).HasDefaultValueSql("((1))");
                entity.Property(e => e.SupplierId).HasMaxLength(6);
                entity.Property(o => o.OrderStatus)
                    .HasConversion(status => (int)status, id => (OrderStatus)id)
                    .HasColumnName("OrderStatusId");

                entity.HasOne(d => d.OrderingPartyContact)
                    .WithMany(p => p.OrderOrderingPartyContacts)
                    .HasForeignKey(d => d.OrderingPartyContactId)
                    .HasConstraintName("FK_Order_OrderingPartyContact");

                entity.HasOne(d => d.OrderingParty)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderingPartyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_OrderingParty");

                entity.HasOne(d => d.SupplierContact)
                    .WithMany(p => p.OrderSupplierContacts)
                    .HasForeignKey(d => d.SupplierContactId)
                    .HasConstraintName("FK_Order_SupplierContact");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_Order_Supplier");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.CatalogueItemId });
                entity.ToTable("OrderItem");
                entity.Property(e => e.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));
                entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasMaxLength(3);
                entity.Property(e => e.DefaultDeliveryDate).HasColumnType("date");
                entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
                entity.Property(e => e.PricingUnitName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.ProvisioningType)
                    .HasColumnName("ProvisioningTypeId")
                    .HasConversion(e => e.Id, e => Models.EnumerationBase.FromId<ProvisioningType>(e));

                entity.HasOne(d => d.CatalogueItem)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.CatalogueItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderItem_CatalogueItem");

                entity.Property(e => e.CataloguePriceType)
                    .HasColumnName("CataloguePriceTypeId")
                    .HasConversion(e => e.Id, e => Models.EnumerationBase.FromId<CataloguePriceType>(e));

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderItem_Order");

                entity.HasOne(d => d.PricingUnit)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.PricingUnitName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderItem_PricingUnit");

                entity.Property(e => e.PriceTimeUnit)
                    .HasColumnName("TimeUnitId")
                    .HasConversion(e => e.Id, e => Models.EnumerationBase.FromId<TimeUnit>(e));

                entity.Property(e => e.EstimationPeriod)
                    .HasColumnName("EstimationPeriodId")
                    .HasConversion(e => e.Id, e => Models.EnumerationBase.FromId<TimeUnit>(e));
            });

            modelBuilder.Entity<OrderItemRecipient>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.CatalogueItemId, e.OdsCode });
                entity.Property(e => e.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));
                entity.Property(e => e.OdsCode).HasMaxLength(8);
                entity.Property(e => e.DeliveryDate).HasColumnType("date");
                entity.HasOne(d => d.OdsCodeNavigation)
                    .WithMany(p => p.OrderItemRecipients)
                    .HasForeignKey(d => d.OdsCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderItemRecipients_OdsCode");

                entity.HasOne(d => d.OrderItem)
                    .WithMany(p => p.OrderItemRecipients)
                    .HasForeignKey(d => new { d.OrderId, d.CatalogueItemId })
                    .HasConstraintName("FK_OrderItemRecipients_OrderItem");
            });

            modelBuilder.Entity<OrderProgress>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.ToTable("OrderProgress");
                entity.Property(e => e.OrderId).ValueGeneratedNever();
                entity.HasOne(d => d.Order)
                    .WithOne(p => p.OrderProgress)
                    .HasForeignKey<OrderProgress>(d => d.OrderId)
                    .HasConstraintName("FK_OrderProgress_Order");
            });

            modelBuilder.Entity<OrderingParty>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);
                entity.ToTable("OrderingParty");
                entity.HasIndex(e => e.OdsCode, "AK_OrderingParty_OdsCode")
                    .IsUnique()
                    .IsClustered();
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Name).HasMaxLength(256);
                entity.Property(e => e.OdsCode).HasMaxLength(8);
                entity.HasOne(d => d.Address)
                    .WithMany(p => p.OrderingParties)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_OrderingParty_Address");
            });

            modelBuilder.Entity<PricingUnit>(entity =>
            {
                entity.HasKey(e => e.Name);
                entity.ToTable("PricingUnit");
                entity.Property(e => e.Name).HasMaxLength(20);
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(40);
            });

            modelBuilder.Entity<SelectedServiceRecipient>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.OdsCode });
                entity.Property(e => e.OdsCode).HasMaxLength(8);
                entity.HasOne(d => d.OdsCodeNavigation)
                    .WithMany(p => p.SelectedServiceRecipients)
                    .HasForeignKey(d => d.OdsCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SelectedServiceRecipients_OdsCode");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.SelectedServiceRecipients)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_SelectedServiceRecipients_Order");
            });

            modelBuilder.Entity<ServiceInstanceItem>(entity =>
            {
                entity.HasKey(i => new { i.OrderId, i.CatalogueItemId, i.OdsCode });
                entity.ToView("ServiceInstanceItem");
                entity.Property(e => e.CatalogueItemId)
                    .IsRequired()
                    .HasMaxLength(14);
                entity.Property(e => e.OdsCode)
                    .IsRequired()
                    .HasMaxLength(8);
                entity.Property(e => e.ServiceInstanceId).HasMaxLength(35);
            });

            modelBuilder.Entity<ServiceRecipient>(entity =>
            {
                entity.HasKey(e => e.OdsCode);
                entity.ToTable("ServiceRecipient");
                entity.Property(e => e.OdsCode).HasMaxLength(8);
                entity.Property(e => e.Name).HasMaxLength(256);
            });

            modelBuilder.Entity<DefaultDeliveryDate>(entity =>
            {
                entity.ToTable("DefaultDeliveryDate");
                entity.HasKey(d => new { d.OrderId, d.CatalogueItemId });
                entity
                    .Property(d => d.CatalogueItemId)
                    .HasMaxLength(14)
                    .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

                entity.Property(d => d.DeliveryDate).HasColumnType("date");

                entity.HasOne<Order>()
                    .WithMany(o => o.DefaultDeliveryDates)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_DefaultDeliveryDate_OrderId");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");
                entity.Property(e => e.Id).HasMaxLength(6);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Supplier_Address");

                entity.HasMany(s => s.Orders)
                    .WithOne(o => o.Supplier)
                    .HasForeignKey(o => o.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
