using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class SlaServiceLevelsEntityTypeConfiguration : IEntityTypeConfiguration<SlaServiceLevel>
    {
        public void Configure(EntityTypeBuilder<SlaServiceLevel> builder)
        {
            builder.ToTable("SlaServiceLevels", Schemas.Catalogue);

            builder.HasKey(s => s.Id);

            builder.Property(s => s.SolutionId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.HasOne(s => s.ServiceLevelAgreement)
                .WithMany(s => s.ServiceLevels)
                .HasForeignKey(s => s.SolutionId)
                .HasConstraintName("FK_SlaServiceLevels_ServiceLevelAgreement");
        }
    }
}
