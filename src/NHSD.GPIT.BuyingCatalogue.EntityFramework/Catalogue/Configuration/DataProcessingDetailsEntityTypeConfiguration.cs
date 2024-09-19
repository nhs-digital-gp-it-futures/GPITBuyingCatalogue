using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public class DataProcessingDetailsEntityTypeConfiguration : IEntityTypeConfiguration<DataProcessingDetails>
{
    public void Configure(EntityTypeBuilder<DataProcessingDetails> builder)
    {
        builder.ToTable("DataProcessingDetails", Schemas.Catalogue);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Subject)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Duration)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.ProcessingNature)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.PersonalDataTypes)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.DataSubjectCategories)
            .IsRequired()
            .HasMaxLength(2000);
    }
}
