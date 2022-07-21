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

            builder.HasOne(x => x.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.LastUpdatedBy)
                .HasConstraintName("FK_DataProcessingPlans_LastUpdatedBy");

            builder.HasMany(x => x.Steps)
                .WithOne(x => x.Plan)
                .HasForeignKey(x => x.PlanId)
                .HasConstraintName("FK_DataProcessingPlanSteps_Plan");
        }
    }
}
