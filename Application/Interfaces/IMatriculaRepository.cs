using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMatriculaRepository : IRepository<Matricula>
    {
        Task<bool> ExisteMatriculaAtivaAsync(int alunoId, int cursoId);
        Task<IEnumerable<Matricula>> GetMatriculasPorAlunoAsync(int alunoId);
        Task<IEnumerable<Matricula>> GetMatriculasPorCursoAsync(int cursoId);
        Task<Matricula> GetMatriculaAtivaPorAlunoECursoAsync(int alunoId, int cursoId);
        Task<IEnumerable<Matricula>> GetMatriculasComDadosRelacionadosAsync();
    }
}