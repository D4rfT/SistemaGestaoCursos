using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MatriculaRepository : BaseRepository<Matricula>, IMatriculaRepository
    {
        public MatriculaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExisteMatriculaAtivaAsync(int alunoId, int cursoId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(m => m.AlunoId == alunoId &&
                              m.CursoId == cursoId &&
                              m.Ativa, cancellationToken);
        }

        public async Task<Matricula> GetMatriculaAtivaPorAlunoECursoAsync(int alunoId, int cursoId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(m => m.AlunoId == alunoId &&
                                         m.CursoId == cursoId &&
                                         m.Ativa, cancellationToken);
        }

        public async Task<IEnumerable<Matricula>> GetMatriculasPorAlunoAsync(int alunoId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.AlunoId == alunoId)
                .OrderByDescending(m => m.DataMatricula)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Matricula>> GetMatriculasPorCursoAsync(int cursoId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.CursoId == cursoId)
                .OrderByDescending(m => m.DataMatricula)
                .ToListAsync(cancellationToken);
        }

        public override async Task<Matricula> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(m => m.Aluno)
                    .ThenInclude(a => a.Curso)
                .Include(m => m.Curso)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Matricula>> GetMatriculasComDadosRelacionadosAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(m => m.Aluno)
                .Include(m => m.Curso)
                .OrderByDescending(m => m.DataMatricula)
                .ToListAsync(cancellationToken);
        }
    }
}