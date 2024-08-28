using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public class DataProtectionSubProcessorEntityTypeConfiguration : IEntityTypeConfiguration<DataProtectionSubProcessor>
{
    public void Configure(EntityTypeBuilder<DataProtectionSubProcessor> builder)
    {
        builder.ToTable("DataProtectionSubProcessors", Schemas.Catalogue);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrganisationName)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.PostProcessingPlan)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne<DataProcessingInformation>()
            .WithMany(x => x.SubProcessors)
            .HasForeignKey(x => x.DataProcessingInfoId)
            .HasConstraintName("FK_DataProtectionSubProcessors_DataProcessingInformation");

        builder.HasOne(x => x.Details)
            .WithOne()
            .HasForeignKey<DataProtectionSubProcessor>(x => x.DataProcessingDetailsId)
            .HasConstraintName("FK_DataProtectionSubProcessors_DataProcessingDetails");
    }
}
