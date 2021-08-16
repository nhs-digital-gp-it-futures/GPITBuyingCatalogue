using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class ServiceInstanceItemEntityTypeConfiguration : IEntityTypeConfiguration<ServiceInstanceItem>
    {
        public void Configure(EntityTypeBuilder<ServiceInstanceItem> builder)
        {
            builder.ToView("ServiceInstanceItems", Schemas.Ordering);
            builder.HasKey(i => new { i.OrderId, i.CatalogueItemId, i.OdsCode });

            builder.Property(e => e.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));
        }
    }
}
