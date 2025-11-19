using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class MatriculaConfiguration : IEntityTypeConfiguration<Matricula>
    {
        public void Configure(EntityTypeBuilder<Matricula> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.AlunoId).IsRequired();

            builder.Property(m => m.CursoId).IsRequired();

            builder.Property(m => m.DataMatricula).IsRequired();

            builder.Property(m => m.Ativa).IsRequired();

            builder.HasOne<Curso>(m => m.Curso)
                .WithMany()
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Aluno>(m => m.Aluno)
                .WithMany()
                .HasForeignKey(m => m.AlunoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(m => new { m.AlunoId, m.CursoId })
                .IsUnique();

        }
    }
}
