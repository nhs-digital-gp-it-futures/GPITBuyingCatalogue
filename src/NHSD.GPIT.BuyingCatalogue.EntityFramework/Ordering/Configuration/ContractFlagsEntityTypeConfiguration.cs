using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class ContractFlagsEntityTypeConfiguration : IEntityTypeConfiguration<ContractFlags>
    {
        public void Configure(EntityTypeBuilder<ContractFlags> builder)
        {
            builder.ToTable("ContractFlags", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_ContractFlags");

            builder.Property(x => x.OrderId).IsRequired();
        }
    }
}
