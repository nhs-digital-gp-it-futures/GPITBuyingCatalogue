using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class WorkOffPlanEntityTypeConfiguration : IEntityTypeConfiguration<WorkOffPlan>
    {
        public void Configure(EntityTypeBuilder<WorkOffPlan> builder)
        {
            builder.ToTable("WorkOffPlans", Schemas.Catalogue);

            builder.HasKey(wop => wop.Id);

            builder.Property(wop => wop.Details)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(wop => wop.SolutionId)
                .IsRequired()
                .HasMaxLength(14);

            builder.Property(wop => wop.StandardId)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(wop => wop.CompletionDate)
                .IsRequired();

            builder.Property(wop => wop.LastUpdated)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(wop => wop.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(wop => wop.LastUpdatedBy)
                .HasConstraintName("FK_WorkOffPlans_LastUpdatedBy");

            builder.HasOne(wop => wop.Solution)
                .WithMany(s => s.WorkOffPlans)
                .HasForeignKey(wop => wop.SolutionId)
                .HasConstraintName("FK_WorkOffPlans_Solution");

            builder.HasOne(wop => wop.Standard)
                .WithMany()
                .HasForeignKey(wop => wop.StandardId)
                .HasConstraintName("FK_WorkOffPlans_Standard");
        }
    }
}
