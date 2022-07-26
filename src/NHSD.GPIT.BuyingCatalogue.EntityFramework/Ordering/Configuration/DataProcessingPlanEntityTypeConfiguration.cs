using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class DataProcessingPlanEntityTypeConfiguration : IEntityTypeConfiguration<DataProcessingPlan>
    {
        public void Configure(EntityTypeBuilder<DataProcessingPlan> builder)
        {
            builder.ToTable("DataProcessingPlans", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_DataProcessingPlans");

            builder.Property(x => x.IsDefault).IsRequired();

            builder.HasMany(x => x.Contracts)
                .WithOne(x => x.DataProcessingPlan)
                .HasForeignKey(x => x.DataProcessingPlanId);
        }
    }
}
