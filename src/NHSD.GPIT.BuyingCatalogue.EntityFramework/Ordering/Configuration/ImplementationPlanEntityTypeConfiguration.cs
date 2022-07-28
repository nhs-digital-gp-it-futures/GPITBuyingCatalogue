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
        }
    }
}
