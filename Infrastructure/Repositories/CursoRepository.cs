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

        public async Task<Curso> GetByNomeAsync(string nome, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Nome.ToLower() == nome.ToLower(), cancellationToken);
        }

        public async Task<IEnumerable<Curso>> GetCursosAtivosAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => c.Ativo)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Curso>> GetCursosComFiltrosAsync(
            string? nome = null,
            decimal? precoMinimo = null,
            decimal? precoMaximo = null,
            int? duracaoMinima = null,
            int? duracaoMaxima = null,
            bool? ativo = null,
            string? ordenarPor = null,
            bool ordemDescendente = false,
            CancellationToken cancellationToken = default)
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

            query = AplicarOrdenacao(query, ordenarPor, ordemDescendente);

            return await query.ToListAsync(cancellationToken);
        }

        private IQueryable<Curso> AplicarOrdenacao(IQueryable<Curso> query, string ordenarPor, bool ordemDescendente)
        {
            if (string.IsNullOrWhiteSpace(ordenarPor))
                return query;

            return ordenarPor.ToLower() switch
            {
                "nome" => ordemDescendente ? query.OrderByDescending(c => c.Nome) : query.OrderBy(c => c.Nome),
                "preco" => ordemDescendente ? query.OrderByDescending(c => c.Preco) : query.OrderBy(c => c.Preco),
                "duracao" => ordemDescendente ? query.OrderByDescending(c => c.Duracao) : query.OrderBy(c => c.Duracao),
                "datacriacao" => ordemDescendente ? query.OrderByDescending(c => c.DataCriacao) : query.OrderBy(c => c.DataCriacao),
                _ => query
            };
        }

    }   
}