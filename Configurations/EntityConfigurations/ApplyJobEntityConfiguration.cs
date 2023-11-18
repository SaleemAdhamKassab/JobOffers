using JobOffers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobOffers.Configurations.EntityConfigurations
{
    public class ApplyJobEntityConfiguration : IEntityTypeConfiguration<ApplyJob>
    {
        public void Configure(EntityTypeBuilder<ApplyJob> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.HasIndex(e => new { e.JobId, e.UserId }).IsUnique(true);
            builder.Property(e => e.Message).IsRequired(false);
            builder.Property(e => e.AddedOn).IsRequired(true).HasDefaultValueSql("getdate()");

            builder.HasOne(e => e.User)
                .WithMany(e => e.ApplyJob)
                .HasForeignKey(e => e.UserId);

            builder.HasOne(e => e.Job)
                .WithMany(e => e.ApplyJob)
                .HasForeignKey(e => e.JobId);
        }
    }
}