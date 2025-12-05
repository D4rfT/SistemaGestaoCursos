using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
    {
        public void Configure(EntityTypeBuilder<Aluno> builder)
        {

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Nome).IsRequired().HasMaxLength(100);

            builder.Property(a => a.CPF).IsRequired().HasMaxLength(11).IsFixedLength();

            builder.Property(a => a.RegistroAcademico).IsRequired().HasMaxLength(20);

            builder.Property(a => a.Email).IsRequired().HasMaxLength(100);

            builder.Property(a => a.DataNascimento).IsRequired();

            builder.Property(a => a.CursoId).IsRequired();

            builder.Property(a => a.UsuarioId).IsRequired(false);

            builder.HasOne(a => a.Curso)
                .WithMany(c => c.Alunos)
                .HasForeignKey(a => a.CursoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => a.CPF).IsUnique();
            builder.HasIndex(a => a.RegistroAcademico).IsUnique();
            builder.HasIndex(a => a.Email).IsUnique();
            builder.HasIndex(a => a.CursoId);

            builder.ToTable("Alunos");
        }
    }
}
