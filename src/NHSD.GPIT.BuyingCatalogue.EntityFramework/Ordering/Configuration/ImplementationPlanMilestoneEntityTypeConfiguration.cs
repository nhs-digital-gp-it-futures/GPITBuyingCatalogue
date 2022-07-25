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
            builder.Property(x => x.PlanId).IsRequired().HasColumnName("ImplementationPlanId");

            builder.Property(x => x.Order).IsRequired();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.PaymentTrigger).IsRequired().HasMaxLength(1000);
        }
    }
}
