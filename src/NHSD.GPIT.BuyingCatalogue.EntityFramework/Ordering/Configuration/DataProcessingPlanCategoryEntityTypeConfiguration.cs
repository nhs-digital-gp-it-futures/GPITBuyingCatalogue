using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class DataProcessingPlanCategoryEntityTypeConfiguration : IEntityTypeConfiguration<DataProcessingPlanCategory>
    {
        public void Configure(EntityTypeBuilder<DataProcessingPlanCategory> builder)
        {
            builder.ToTable("DataProcessingPlanCategories", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_DataProcessingPlanCategories");

            builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);

            builder.HasOne(x => x.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.LastUpdatedBy)
                .HasConstraintName("FK_DataProcessingPlanCategories_LastUpdatedBy");
        }
    }
}
