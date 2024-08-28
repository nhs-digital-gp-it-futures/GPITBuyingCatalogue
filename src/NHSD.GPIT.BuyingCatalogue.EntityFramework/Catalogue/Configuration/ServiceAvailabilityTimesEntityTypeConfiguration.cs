using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class ServiceAvailabilityTimesEntityTypeConfiguration : IEntityTypeConfiguration<ServiceAvailabilityTimes>
    {
        public void Configure(EntityTypeBuilder<ServiceAvailabilityTimes> builder)
        {
            builder.ToTable("ServiceAvailabilityTimes", Schemas.Catalogue, b => b.IsTemporal(
                temp =>
                {
                    temp.UseHistoryTable("ServiceAvailabilityTimes_History");
                    temp.HasPeriodStart("SysStartTime");
                    temp.HasPeriodEnd("SysEndTime");
                }));

            builder.HasKey(s => s.Id);

            builder.Property(s => s.SolutionId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(x => x.IncludedDays)
                .IsRequired(false)
                .HasConversion(
                    v => string.Join(',', v.Select(x => x.ToString("D")).ToArray()),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Enum.Parse<Iso8601DayOfWeek>).ToArray());

            builder.Property(x => x.IncludesBankHolidays)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.AdditionalInformation)
                .IsRequired(false);

            builder.Property(s => s.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(s => s.ServiceLevelAgreement)
                .WithMany(s => s.ServiceHours)
                .HasForeignKey(s => s.SolutionId)
                .HasConstraintName("FK_ServiceAvailabilityTimes_ServiceLevelAgreements");

            builder.HasOne(s => s.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(s => s.LastUpdatedBy)
                .HasConstraintName("FK_ServiceAvailabilityTimes_LastUpdatedBy");
        }
    }
}
