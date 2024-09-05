using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public class DataProcessingInformationEntityTypeConfiguration : IEntityTypeConfiguration<DataProcessingInformation>
{
    public void Configure(EntityTypeBuilder<DataProcessingInformation> builder)
    {
        builder.ToTable("DataProcessingInformation", Schemas.Catalogue);

        builder.HasKey(x => x.Id);

        builder.HasOne<Solution>()
            .WithOne(x => x.DataProcessingInformation)
            .HasForeignKey<DataProcessingInformation>(x => x.SolutionId)
            .HasConstraintName("FK_DataProcessingInformation_Solution");

        builder.HasOne(x => x.Details)
            .WithOne()
            .HasForeignKey<DataProcessingInformation>(x => x.DataProcessingDetailsId)
            .HasConstraintName("FK_DataProcessingInformation_DataProcessingDetails");
    }
}
