using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class MuseumConfuguration : IEntityTypeConfiguration<Museum>
    {
        public void Configure(EntityTypeBuilder<Museum> builder)
        {
            builder.ToTable("Museum");

            builder.Property(e => e.CreatedAt).HasColumnType("datetime");
            builder.Property(e => e.DeletedAt).HasColumnType("datetime");
            builder.Property(e => e.Name).HasMaxLength(100);
            builder.Property(e => e.UpdatedAt).HasColumnType("datetime");
        }
    }
}
