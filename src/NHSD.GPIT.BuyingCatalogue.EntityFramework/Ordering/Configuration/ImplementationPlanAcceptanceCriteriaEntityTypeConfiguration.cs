using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal class ImplementationPlanAcceptanceCriteriaEntityTypeConfiguration : IEntityTypeConfiguration<ImplementationPlanAcceptanceCriteria>
    {
        public void Configure(EntityTypeBuilder<ImplementationPlanAcceptanceCriteria> builder)
        {
            builder.ToTable("ImplementationPlanAcceptanceCriteria", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_ImplementationPlanAcceptanceCriteria");
            builder.Property(x => x.MilestoneId).IsRequired().HasColumnName("ImplementationPlanMilestoneId");

            builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        }
    }
}
