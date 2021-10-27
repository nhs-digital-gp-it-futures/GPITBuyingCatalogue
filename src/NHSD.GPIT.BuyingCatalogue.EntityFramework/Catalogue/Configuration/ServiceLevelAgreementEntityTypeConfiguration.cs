using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class ServiceLevelAgreementEntityTypeConfiguration : IEntityTypeConfiguration<ServiceLevelAgreements>
    {
        public void Configure(EntityTypeBuilder<ServiceLevelAgreements> builder)
        {
            builder.ToTable("ServiceLevelAgreements", Schemas.Catalogue);

            builder.HasKey(s => s.SolutionId);

            builder.Property(s => s.SolutionId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.HasOne(s => s.Solution)
                .WithOne(s => s.ServiceLevelAgreement)
                .HasForeignKey<ServiceLevelAgreements>(s => s.SolutionId)
                .HasConstraintName("FK_ServiceLevelAgreements_Solution");
        }
    }
}
