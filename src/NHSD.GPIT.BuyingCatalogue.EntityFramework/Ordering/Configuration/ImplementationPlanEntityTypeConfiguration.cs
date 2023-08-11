using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class ImplementationPlanEntityTypeConfiguration : IEntityTypeConfiguration<ImplementationPlan>
    {
        public void Configure(EntityTypeBuilder<ImplementationPlan> builder)
        {
            builder.ToTable("ImplementationPlans", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_ImplementationPlans");

            builder.Property(x => x.IsDefault).IsRequired();

            builder.Property(x => x.ContractId);

            builder.HasOne(x => x.Contract)
                .WithOne(y => y.ImplementationPlan)
                .HasForeignKey<ImplementationPlan>(x => x.ContractId)
                .HasConstraintName("FK_ImplementationPlans_Contract")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.LastUpdatedBy)
                .HasConstraintName("FK_ImplementationPlans_LastUpdatedBy");

            builder.HasMany(x => x.Milestones)
                .WithOne(x => x.Plan)
                .HasForeignKey(x => x.PlanId)
                .HasConstraintName("FK_ImplementationPlanMilestones_ImplementationPlan");
        }
    }
}
