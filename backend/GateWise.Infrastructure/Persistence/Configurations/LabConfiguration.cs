using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GateWise.Core.Entities;

public class LabConfiguration : IEntityTypeConfiguration<Lab>
{
    public void Configure(EntityTypeBuilder<Lab> builder)
    {
        builder.ToTable("labs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name).IsRequired().HasMaxLength(100);
        builder.Property(l => l.Code).IsRequired().HasMaxLength(50);
        builder.Property(l => l.ImagemUrl).HasMaxLength(255);
        builder.Property(l => l.Description).HasMaxLength(500);
        builder.Property(l => l.Location).HasMaxLength(100);
        builder.Property(l => l.Building).HasMaxLength(100);

        builder.Property(l => l.OpenTime).IsRequired();
        builder.Property(l => l.CloseTime).IsRequired();
        builder.Property(l => l.IsActive).IsRequired();

        builder.Property(l => l.CreatedAt).IsRequired();
        builder.Property(l => l.UpdatedAt).IsRequired();
    }
}
