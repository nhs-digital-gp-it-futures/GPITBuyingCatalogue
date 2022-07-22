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
            builder.Property(x => x.PlanId).IsRequired().HasField("DataProcessingPlanId");
            builder.Property(x => x.CategoryId).IsRequired().HasField("DataProcessingPlanCategoryId");

            builder.Property(x => x.Details).IsRequired().HasMaxLength(1000);

            builder.HasOne(x => x.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.LastUpdatedBy)
                .HasConstraintName("FK_DataProcessingPlanSteps_LastUpdatedBy");

            builder.HasOne(x => x.Plan)
                .WithMany()
                .HasForeignKey(x => x.PlanId)
                .HasConstraintName("FK_DataProcessingPlanSteps_Plan");

            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .HasConstraintName("FK_DataProcessingPlanSteps_Category");
        }
    }
}
