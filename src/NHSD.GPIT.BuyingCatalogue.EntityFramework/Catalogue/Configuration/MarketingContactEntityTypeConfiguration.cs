using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class MarketingContactEntityTypeConfiguration : IEntityTypeConfiguration<MarketingContact>
    {
        public void Configure(EntityTypeBuilder<MarketingContact> builder)
        {
            builder.ToTable("MarketingContacts", Schemas.Catalogue);

            builder.Property(m => m.SolutionId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(m => m.Id).ValueGeneratedOnAdd();
            builder.Property(m => m.Department).HasMaxLength(50);
            builder.Property(m => m.Email).HasMaxLength(255);
            builder.Property(m => m.FirstName).HasMaxLength(35);
            builder.Property(m => m.LastName).HasMaxLength(35);
            builder.Property(m => m.PhoneNumber).HasMaxLength(35);

            builder.HasOne<Solution>()
                .WithMany(s => s.MarketingContacts)
                .HasForeignKey(m => m.SolutionId)
                .HasConstraintName("FK_MarketingContact_Solution");

            builder.HasOne(m => m.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(m => m.LastUpdatedBy)
                .HasConstraintName("FK_MarketingContacts_LastUpdatedBy");
        }
    }
}
