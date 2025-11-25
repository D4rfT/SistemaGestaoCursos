using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICursoRepository : IRepository<Curso>
    {
        Task<Curso> GetByNomeAsync(string nome, CancellationToken cancellationToken = default);

        Task<IEnumerable<Curso>> GetCursosAtivosAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Curso>> GetCursosComFiltrosAsync(
            string? nome = null,
            decimal? precoMinimo = null,
            decimal? precoMaximo = null,
            int? duracaoMinima = null,
            int? duracaoMaxima = null,
            bool? ativo = null,
            string? ordenarPor = null,
            bool ordemDescendente = false,
            CancellationToken cancellationToken = default);
    }
}