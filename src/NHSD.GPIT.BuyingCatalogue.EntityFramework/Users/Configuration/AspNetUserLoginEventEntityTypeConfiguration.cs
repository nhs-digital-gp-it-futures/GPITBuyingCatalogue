using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Configuration;

public class AspNetUserLoginEventEntityTypeConfiguration : IEntityTypeConfiguration<AspNetUserLoginEvent>
{
    public void Configure(EntityTypeBuilder<AspNetUserLoginEvent> builder)
    {
        builder.ToTable("AspNetUserLoginEvents", Schemas.Users);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.Date)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithMany(u => u.LoginEvents)
            .HasForeignKey(e => e.UserId);

        builder.HasIndex(e => new { e.UserId, e.Date })
            .HasDatabaseName("IX_AspNetUserLoginEvents_UserId_Date");

        builder.HasIndex(e => e.Date)
            .HasDatabaseName("IX_AspNetUserLoginEvents_Date");
    }
}
