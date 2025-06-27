using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GateWise.Core.Entities;

public class AccessGrantConfiguration : IEntityTypeConfiguration<AccessGrant>
{
    public void Configure(EntityTypeBuilder<AccessGrant> builder)
    {
        builder.ToTable("access_grants");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Reason).HasMaxLength(255);

        builder.Property(a => a.Status)
               .HasConversion<string>()
               .IsRequired();

        builder.HasOne(a => a.AuthorizedUser)
               .WithMany()
               .HasForeignKey(a => a.AuthorizedUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.GrantedByUser)
               .WithMany()
               .HasForeignKey(a => a.GrantedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); 


        builder.HasOne(a => a.Lab)
               .WithMany()
               .HasForeignKey(a => a.LabId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
