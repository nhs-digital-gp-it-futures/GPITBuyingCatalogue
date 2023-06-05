﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class RequiredServiceEntityTypeConfiguration : IEntityTypeConfiguration<RequiredService>
{
    public void Configure(EntityTypeBuilder<RequiredService> builder)
    {
        builder.ToTable("RequiredServices", Schemas.Competitions);

        builder.HasKey(x => new { x.CompetitionId, x.SolutionId, x.ServiceId });

        builder.Property(x => x.SolutionId)
            .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

        builder.Property(x => x.ServiceId)
            .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

        builder.HasOne<Competition>()
            .WithMany()
            .HasForeignKey(x => x.CompetitionId)
            .HasConstraintName("FK_RequiredServices_Competition");

        builder.HasOne<CompetitionSolution>()
            .WithMany()
            .HasForeignKey(x => new { x.CompetitionId, x.SolutionId })
            .HasConstraintName("FK_RequiredServices_Solution");

        builder.HasOne(x => x.Service)
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .HasConstraintName("FK_RequiredServices_Service");
    }
}
