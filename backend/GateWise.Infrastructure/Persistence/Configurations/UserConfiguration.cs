using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GateWise.Core.Entities;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
       public void Configure(EntityTypeBuilder<User> builder)
       {
              builder.ToTable("users");

              builder.HasKey(u => u.Id);

              builder.Property(u => u.Id)
                  .HasColumnType("varchar(36)")
                  .IsRequired();

              builder.Property(u => u.Name)
                     .IsRequired()
                     .HasMaxLength(100);

              builder.Property(u => u.Email)
                     .IsRequired()
                     .HasMaxLength(100);

              builder.Property(u => u.RegistrationNumber)
                     .IsRequired()
                     .HasMaxLength(20);

              builder.Property(u => u.DevicePublicKeyPem)
              .HasColumnType("text") 
              .IsRequired(false);

              builder.Property(u => u.UserAvatarUrl)
                     .HasMaxLength(255);
       }
}
