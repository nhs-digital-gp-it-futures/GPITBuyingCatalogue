using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework
{
    public partial class GPITBuyingCatalogueDbContext
    {
        private static void ApplyOrderingConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ContactEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DefaultDeliveryDateEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemRecipientEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProgressEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceRecipientEntityTypeConfiguration());

            modelBuilder.ApplyConfiguration(new ServiceInstanceItemEntityTypeConfiguration());
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            ApplyOrderingConfiguration(modelBuilder);
        }
    }
}
