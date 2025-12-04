using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Nome).IsRequired().HasMaxLength(100);

            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);

            builder.Property(u => u.SenhaHash).IsRequired().HasMaxLength(255);

            builder.Property(u => u.Role).IsRequired().HasMaxLength(50);

            builder.Property(u => u.Ativo).IsRequired().HasDefaultValue(true);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.ToTable("Usuarios");
        }
    }
}