using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackEndAPI.Models
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("AppUsers");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Password).IsRequired().HasMaxLength(100);
            builder.Property(u => u.UpdateDate)
                   .IsRequired()
                   .HasConversion(
                        v => v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(u => u.CreatedDate)
                   .IsRequired()
                   .HasConversion(
                        v => v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            builder.Property(u => u.Token).HasMaxLength(512);
        }
    }
}
