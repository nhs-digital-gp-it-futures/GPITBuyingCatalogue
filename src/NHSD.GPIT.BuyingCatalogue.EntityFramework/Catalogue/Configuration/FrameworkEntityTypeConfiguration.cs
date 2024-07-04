using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class FrameworkEntityTypeConfiguration : IEntityTypeConfiguration<Framework>
    {
        public void Configure(EntityTypeBuilder<Framework> builder)
        {
            builder.ToTable("Frameworks", Schemas.Catalogue);

            builder.Property(f => f.Id).HasDefaultValue(Guid.NewGuid().ToString()).HasMaxLength(36);

            builder.Property(f => f.ShortName).HasMaxLength(100);
            builder.Property(f => f.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.Property(f => f.IsExpired);

            builder.Property(x => x.FundingTypes)
                .HasConversion(
                    v => string.Join(',', v.Select(x => x.ToString("D")).ToArray()),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Enum.Parse<FundingType>).ToArray());

            builder.Ignore(x => x.HasSingleFundingType);

            builder.HasOne(f => f.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(f => f.LastUpdatedBy)
                .HasConstraintName("FK_Frameworks_LastUpdatedBy");
        }
    }
}
