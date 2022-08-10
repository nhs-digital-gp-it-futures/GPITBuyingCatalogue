using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Configuration;

public class EmailDomainEntityTypeConfiguration : IEntityTypeConfiguration<EmailDomain>
{
    public void Configure(EntityTypeBuilder<EmailDomain> builder)
    {
        builder.ToTable("EmailDomains", Schemas.Users);

        builder.Property(d => d.Id).ValueGeneratedOnAdd();

        builder.Property(d => d.Domain).IsRequired().HasMaxLength(50);
    }
}
