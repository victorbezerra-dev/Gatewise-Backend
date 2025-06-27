using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GateWise.Core.Entities;

public class LabAccessManagerConfiguration : IEntityTypeConfiguration<LabAccessManager>
{
    public void Configure(EntityTypeBuilder<LabAccessManager> builder)
    {
        builder.ToTable("lab_access_managers");

        builder.HasKey(l => l.Id);

        builder.HasOne(l => l.Lab)
               .WithMany()
               .HasForeignKey(l => l.LabId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.User)
               .WithMany()
               .HasForeignKey(l => l.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
