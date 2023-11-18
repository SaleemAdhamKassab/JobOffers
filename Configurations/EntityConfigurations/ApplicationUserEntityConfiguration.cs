using JobOffers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static JobOffers.Configurations.Helper;

namespace JobOffers.Configurations.EntityConfigurations
{
    public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(e => e.FirstName).HasMaxLength(50).IsRequired(true);
            builder.Property(e => e.LastName).HasMaxLength(50).IsRequired(true);
            builder.Property(e => e.Image).HasDefaultValue(UploadFile.UserDefaultImage).IsRequired(false);
            builder.Property(e => e.CV).IsRequired(false);
        }
    }
}