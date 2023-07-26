using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class ContractEntityTypeConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("Contracts", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_Contracts");

            builder.Property(x => x.OrderId).IsRequired();

            builder.HasOne(x => x.Order)
                .WithOne(x => x.Contract)
                .HasForeignKey<Contract>(x => x.OrderId);
        }
    }
}
