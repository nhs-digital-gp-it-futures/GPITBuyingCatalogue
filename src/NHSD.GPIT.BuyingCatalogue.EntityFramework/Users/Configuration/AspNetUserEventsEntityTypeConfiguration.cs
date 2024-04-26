using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Configuration;

public class AspNetUserEventsEntityTypeConfiguration : IEntityTypeConfiguration<AspNetUserEvent>
{
    public void Configure(EntityTypeBuilder<AspNetUserEvent> builder)
    {
        builder.ToTable("AspNetUserEvents", Schemas.Users);

        builder.HasKey(x => x.Id).HasName("PK_AspNetUserEvents");

        builder.HasOne(x => x.EventType)
            .WithMany()
            .HasForeignKey(x => x.EventTypeId)
            .HasConstraintName("FK_AspNetUserEvents_EventType");
    }
}
