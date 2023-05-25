using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

internal sealed class CompetitionEntityTypeConfiguration : IEntityTypeConfiguration<Competition>
{
    public void Configure(EntityTypeBuilder<Competition> builder)
    {
        builder.ToTable(
            "Competitions",
            Schemas.Competitions,
            b => b.IsTemporal(
                temp =>
                {
                    temp.UseHistoryTable("Competitions_History");
                    temp.HasPeriodStart("SysStartTime");
                    temp.HasPeriodEnd("SysEndTime");
                }));

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(255);

        builder.Property(x => x.Description).HasMaxLength(250);

        builder.Property(x => x.LastUpdated).HasDefaultValue(DateTime.UtcNow);

        builder.Property(x => x.Completed);

        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasOne(x => x.Filter)
            .WithMany()
            .HasForeignKey(x => x.FilterId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Competitions_Filter");

        builder.HasOne(x => x.Organisation)
            .WithMany()
            .HasForeignKey(x => x.OrganisationId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Competitions_Organisation");

        builder.HasOne(x => x.LastUpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.LastUpdatedBy)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Competitions_LastUpdatedBy");
    }
}