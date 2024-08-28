using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public class DataProcessingLocationEntityTypeConfiguration : IEntityTypeConfiguration<DataProcessingLocation>
{
    public void Configure(EntityTypeBuilder<DataProcessingLocation> builder)
    {
        builder.ToTable("DataProcessingLocation", Schemas.Catalogue);

        builder.HasKey(x => x.DataProcessingInfoId);

        builder.Property(x => x.Location)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.AdditionalJurisdiction)
            .HasMaxLength(2000);

        builder.HasOne<DataProcessingInformation>()
            .WithOne(x => x.Location)
            .HasForeignKey<DataProcessingLocation>(x => x.DataProcessingInfoId)
            .HasConstraintName("FK_DataProcessingLocation_DataProcessingInformation");
    }
}
