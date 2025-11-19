using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CursoRepository : BaseRepository<Curso>, ICursoRepository
    {
        public CursoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Curso> GetByNomeAsync(string nome)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Nome.ToLower() == nome.ToLower());
        }

        public async Task<IEnumerable<Curso>> GetCursosAtivosAsync()
        {
            return await _dbSet.Where(c => c.Ativo).ToListAsync();
        }

        public async Task<IEnumerable<Curso>> GetCursosComFiltrosAsync(
            string? nome = null,
            decimal? precoMinimo = null,
            decimal? precoMaximo = null,
            int? duracaoMinima = null,
            int? duracaoMaxima = null,
            bool? ativo = null)
        {
            IQueryable<Curso> query = _dbSet;

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(c => c.Nome.ToLower().Contains(nome.ToLower()));

            if (precoMinimo.HasValue)
                query = query.Where(c => c.Preco >= precoMinimo.Value);

            if (precoMaximo.HasValue)
                query = query.Where(c => c.Preco <= precoMaximo.Value);

            if (duracaoMinima.HasValue)
                query = query.Where(c => c.Duracao >= duracaoMinima.Value);

            if (duracaoMaxima.HasValue)
                query = query.Where(c => c.Duracao <= duracaoMaxima.Value);

            if (ativo.HasValue)
                query = query.Where(c => c.Ativo == ativo.Value);

            return await query.ToListAsync();
        }
    }
}