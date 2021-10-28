using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class SlaServiceHoursEntityTypeConfiguration : IEntityTypeConfiguration<SlaServiceHours>
    {
        public void Configure(EntityTypeBuilder<SlaServiceHours> builder)
        {
            builder.ToTable("SlaServiceHours", Schemas.Catalogue);

            builder.HasKey(s => s.Id);

            builder.Property(s => s.SolutionId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.HasOne(s => s.ServiceLevelAgreement)
                .WithMany(s => s.ServiceHours)
                .HasForeignKey(s => s.SolutionId)
                .HasConstraintName("FK_SlaServiceHours_ServiceLevelAgreement");
        }
    }
}
