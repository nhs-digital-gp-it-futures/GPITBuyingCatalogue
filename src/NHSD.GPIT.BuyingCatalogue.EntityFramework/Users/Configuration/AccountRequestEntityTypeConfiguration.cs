using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Configuration;

public class AccountRequestEntityTypeConfiguration : IEntityTypeConfiguration<AccountRequest>
{
    public void Configure(EntityTypeBuilder<AccountRequest> builder)
    {
        builder.ToTable("AccountRequests", Schemas.Users);

        builder.HasQueryFilter(x => x.IsConfirmed);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Ignore(x => x.FullName);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.RequestJustification)
            .HasMaxLength(1500)
            .IsRequired();

        builder.Property(x => x.DecisionJustification)
            .HasMaxLength(1500);

        builder.Property(x => x.OdsCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.RequestedOn)
            .HasDefaultValue(DateTime.UtcNow);

        builder.HasOne(x => x.Organisation)
            .WithMany()
            .HasForeignKey(x => x.OdsCode)
            .HasConstraintName("FK_AccountRequests_Organisation");

        builder.HasOne(x => x.DecidedByUser)
            .WithMany()
            .HasForeignKey(x => x.DecidedBy)
            .HasConstraintName("FK_AccountRequests_DecidedBy");
    }
}
