using JobOffers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobOffers.Configurations.EntityConfigurations
{
    public class JobEntityConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.HasIndex(e => new { e.UserId, e.Title, e.AddedOn }).IsUnique(true);
            builder.Property(e => e.Title).HasMaxLength(100).IsRequired(true);
            builder.Property(e => e.CompanyName).HasMaxLength(100).IsRequired(true);
            builder.Property(e => e.Description).IsRequired(true);
            builder.Property(e => e.Image).IsRequired(true);
            builder.Property(e => e.AddedOn).IsRequired(true).HasDefaultValueSql("getdate()");


            builder.HasOne(e => e.Category)
                .WithMany(e => e.Jobs)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.User)
                .WithMany(e => e.Jobs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
