using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class DataProcessingPlanStepEntityTypeConfiguration : IEntityTypeConfiguration<DataProcessingPlanStep>
    {
        public void Configure(EntityTypeBuilder<DataProcessingPlanStep> builder)
        {
            builder.ToTable("DataProcessingPlanSteps", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_DataProcessingPlanSteps");
            builder.Property(x => x.PlanId).IsRequired().HasColumnName("DataProcessingPlanId");
            builder.Property(x => x.CategoryId).IsRequired().HasColumnName("DataProcessingPlanCategoryId");

            builder.Property(x => x.Details).IsRequired().HasMaxLength(1000);

            builder.HasOne(x => x.Category)
                .WithOne();
        }
    }
}
