using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class CompetitionItemQuantityEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionItemQuantity>
{
    public void Configure(EntityTypeBuilder<CompetitionItemQuantity> builder)
    {
        builder.ToTable("CompetitionItemQuantities", Schemas.Competitions);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompetitionId).IsRequired();

        builder.Property(x => x.RecipientOdsCode).HasMaxLength(10).IsRequired();

        builder.Property(x => x.ParentSublocationOdsCode).HasMaxLength(10).IsRequired();

        builder.HasOne(x => x.Recipient)
            .WithMany()
            .HasForeignKey(x => new { x.CompetitionId, x.ParentSublocationOdsCode, x.RecipientOdsCode })
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_CompetitionItemQuantities_Recipient");
    }
}
