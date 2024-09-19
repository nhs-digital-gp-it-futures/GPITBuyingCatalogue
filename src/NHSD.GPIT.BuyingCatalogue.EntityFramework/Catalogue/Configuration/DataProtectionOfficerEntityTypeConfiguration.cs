using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public class DataProtectionOfficerEntityTypeConfiguration : IEntityTypeConfiguration<DataProtectionOfficer>
{
    public void Configure(EntityTypeBuilder<DataProtectionOfficer> builder)
    {
        builder.ToTable("DataProtectionOfficers", Schemas.Catalogue);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.EmailAddress)
            .HasMaxLength(256);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(35);

        builder.HasOne<DataProcessingInformation>()
            .WithOne(x => x.Officer)
            .HasForeignKey<DataProtectionOfficer>(x => x.DataProcessingInfoId)
            .HasConstraintName("FK_DataProtectionOfficers_DataProcessingInformation");
    }
}
