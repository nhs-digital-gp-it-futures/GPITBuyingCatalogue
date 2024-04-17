using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class ContractOrderNumberEntityTypeConfiguration : IEntityTypeConfiguration<ContractOrderNumber>
    {
        public void Configure(EntityTypeBuilder<ContractOrderNumber> builder)
        {
            builder.ToSqlQuery("Select distinct ordernumber from ordering.orders");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("ordernumber");

            builder.HasMany(x => x.OrderEvents)
                .WithOne()
                .HasForeignKey(x => x.OrderNumber)
                .IsRequired(false);
        }
    }
}
