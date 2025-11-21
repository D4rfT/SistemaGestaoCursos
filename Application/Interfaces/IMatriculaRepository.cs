using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMatriculaRepository : IRepository<Matricula>
    {
        Task<bool> ExisteMatriculaAtivaAsync(int alunoId, int cursoId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Matricula>> GetMatriculasPorAlunoAsync(int alunoId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Matricula>> GetMatriculasPorCursoAsync(int cursoId, CancellationToken cancellationToken = default);

        Task<Matricula> GetMatriculaAtivaPorAlunoECursoAsync(int alunoId, int cursoId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Matricula>> GetMatriculasComDadosRelacionadosAsync(CancellationToken cancellationToken = default);
    }
}