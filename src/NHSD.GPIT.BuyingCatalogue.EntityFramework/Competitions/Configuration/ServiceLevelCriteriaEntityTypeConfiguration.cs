using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class ServiceLevelCriteriaEntityTypeConfiguration : IEntityTypeConfiguration<ServiceLevelCriteria>
{
    public void Configure(EntityTypeBuilder<ServiceLevelCriteria> builder)
    {
        builder.ToTable("ServiceLevelCriteria", Schemas.Competitions);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TimeFrom).IsRequired();

        builder.Property(x => x.TimeUntil).IsRequired();

        builder.Property(x => x.ApplicableDays).IsRequired().HasMaxLength(1000);
    }
}
