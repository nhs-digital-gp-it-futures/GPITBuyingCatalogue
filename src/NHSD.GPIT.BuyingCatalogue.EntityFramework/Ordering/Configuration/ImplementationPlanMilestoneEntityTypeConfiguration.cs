using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class ImplementationPlanMilestoneEntityTypeConfiguration : IEntityTypeConfiguration<ImplementationPlanMilestone>
    {
        public void Configure(EntityTypeBuilder<ImplementationPlanMilestone> builder)
        {
            builder.ToTable("ImplementationPlanMilestones", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_ImplementationPlanMilestones");
            builder.Property(x => x.PlanId).IsRequired();

            builder.Property(x => x.Order).IsRequired();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);

            builder.HasOne(x => x.Plan)
                .WithMany()
                .HasForeignKey(x => x.PlanId)
                .HasConstraintName("FK_ImplementationPlanMilestones_Plan");

            builder.HasOne(x => x.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.LastUpdatedBy)
                .HasConstraintName("FK_ImplementationPlanMilestones_LastUpdatedBy");

            builder.HasMany(x => x.AcceptanceCriteria)
                .WithOne(x => x.Milestone)
                .HasForeignKey(x => x.MilestoneId)
                .HasConstraintName("FK_ImplementationPlanAcceptanceCriteria_Milestone");
        }
    }
}
