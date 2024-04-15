using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Configuration
{
    internal sealed class AspNetUserEntityTypeConfiguration : IEntityTypeConfiguration<AspNetUser>
    {
        public void Configure(EntityTypeBuilder<AspNetUser> builder)
        {
            builder.ToTable("AspNetUsers", Schemas.Users, b => b.IsTemporal(
                temp =>
                {
                    temp.UseHistoryTable("AspNetUsers_History");
                    temp.HasPeriodStart("SysStartTime");
                    temp.HasPeriodEnd("SysEndTime");
                }));

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(u => u.CatalogueAgreementSigned).IsRequired().HasDefaultValue(0);
            builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
            builder.Property(u => u.EmailConfirmed).IsRequired().HasDefaultValue(0);
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Ignore(u => u.FullName);

            builder.Property(u => u.NormalizedEmail).HasMaxLength(256).IsRequired();
            builder.Property(u => u.NormalizedUserName).HasMaxLength(256).IsRequired();

            builder.Property(u => u.PhoneNumber).HasMaxLength(35);
            builder.Property(u => u.PhoneNumberConfirmed).IsRequired().HasDefaultValue(0);
            builder.Property(u => u.UserName).HasMaxLength(256).IsRequired();
            builder.Property(u => u.PasswordUpdatedDate).HasDefaultValue(DateTime.UtcNow);
            builder.Property(u => u.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(u => u.PrimaryOrganisation)
                .WithMany()
                .HasForeignKey(u => u.PrimaryOrganisationId)
                .HasConstraintName("FK_AspNetUsers_OrganisationId");

            builder.HasIndex(u => u.NormalizedEmail, "AK_AspNetUsers_NormalizedEmail").IsUnique();
            builder.HasIndex(u => u.NormalizedUserName, "AK_AspNetUsers_NormalizedUserName")
                .IsUnique();

            builder.HasOne(u => u.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(u => u.LastUpdatedBy)
                .HasConstraintName("FK_AspNetUsers_LastUpdatedBy");

            builder.Property(u => u.AcceptedTermsOfUseDate);
            builder.Property(u => u.HasOptedInUserResearch);

            builder.HasMany(x => x.Events)
                .WithOne()
                .HasForeignKey(x => x.UserId);
        }
    }
}
