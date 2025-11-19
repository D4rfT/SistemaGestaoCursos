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

        public async Task<bool> ExisteMatriculaAtivaAsync(int alunoId, int cursoId)
        {
            return await _dbSet.AnyAsync(m => m.AlunoId == alunoId && m.CursoId == cursoId && m.Ativa);
        }

        public async Task<Matricula> GetMatriculaAtivaPorAlunoECursoAsync(int alunoId, int cursoId)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.AlunoId == alunoId && m.CursoId == cursoId && m.Ativa);
        }

        public async Task<IEnumerable<Matricula>> GetMatriculasPorAlunoAsync(int alunoId)
        {
            return await _dbSet.Where(m => m.AlunoId == alunoId).ToListAsync();
        }

        public async Task<IEnumerable<Matricula>> GetMatriculasPorCursoAsync(int cursoId)
        {
            return await _dbSet.Where(m => m.CursoId == cursoId).OrderByDescending(m => m.CursoId).ToListAsync();
        }

        public override async Task<Matricula> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Aluno)
                    .ThenInclude(a => a.Curso)
                .Include(m => m.Curso)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Matricula>> GetMatriculasComDadosRelacionadosAsync()
        {
            return await _dbSet
                .Include(m => m.Aluno)
                .Include(m => m.Curso)
                .OrderByDescending(m => m.DataMatricula)
                .ToListAsync();
        }
    }
}
