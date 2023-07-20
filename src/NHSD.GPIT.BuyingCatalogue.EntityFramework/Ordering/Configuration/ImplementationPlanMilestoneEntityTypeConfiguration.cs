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
            builder.Property(x => x.PlanId).HasColumnName("ImplementationPlanId");
            builder.Property(x => x.ContractBillingItemId);

            builder.Property(x => x.Order).IsRequired();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.PaymentTrigger).IsRequired().HasMaxLength(1000);

            builder.HasOne(x => x.Plan)
                .WithMany(x => x.Milestones)
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ContractBillingItem)
                .WithOne(y => y.Milestone)
                .HasForeignKey<ImplementationPlanMilestone>(x => x.ContractBillingItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.LastUpdatedBy)
                .HasConstraintName("FK_ImplementationPlanMilestones_LastUpdatedBy");
        }
    }
}
