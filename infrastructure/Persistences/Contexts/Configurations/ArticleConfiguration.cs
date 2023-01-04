using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Article");

            builder.Property(e => e.CreatedAt).HasColumnType("datetime");
            builder.Property(e => e.DeletedAt).HasColumnType("datetime");
            builder.Property(e => e.Name).HasMaxLength(100);
            builder.Property(e => e.UpdatedAt).HasColumnType("datetime");

            builder.HasOne(d => d.IdMuseumNavigation).WithMany(p => p.Articles)
                .HasForeignKey(d => d.IdMuseum)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Article__IdMuseu__5165187F");
        }
    }
}
