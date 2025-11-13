using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CursoConfiguration : IEntityTypeConfiguration<Curso>
    {
        public void Configure(EntityTypeBuilder<Curso> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);

            builder.Property(c => c.Descricao).IsRequired().HasMaxLength(500);

            builder.Property(c => c.Preco).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(c => c.Duracao).IsRequired();

            builder.Property(c => c.Ativo).IsRequired();

            builder.Property(c => c.DataCriacao).IsRequired();

            builder.HasIndex(c => c.Nome);
            builder.HasIndex(c => c.Ativo);

            builder.ToTable("Cursos");
        }
    }
}