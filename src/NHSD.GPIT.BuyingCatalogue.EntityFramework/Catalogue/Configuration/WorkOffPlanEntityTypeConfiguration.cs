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

            builder.HasKey(wp => wp.Id);

            builder.Property(wp => wp.Details)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(wp => wp.SolutionId)
                .IsRequired()
                .HasMaxLength(14);

            builder.Property(wp => wp.StandardId)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(wp => wp.CompletionDate)
                .IsRequired();

            builder.Property(wp => wp.LastUpdated)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(wp => wp.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(wp => wp.LastUpdatedBy)
                .HasConstraintName("FK_WorkOffPlans_LastUpdatedBy");

            builder.HasOne(wp => wp.Solution)
                .WithMany(s => s.WorkOffPlans)
                .HasForeignKey(wp => wp.SolutionId)
                .HasConstraintName("FK_WorkOffPlans_Solution");

            builder.HasOne(wp => wp.Standard)
                .WithMany()
                .HasForeignKey(wp => wp.StandardId)
                .HasConstraintName("FK_WorkOffPlans_Standard");
        }
    }
}
