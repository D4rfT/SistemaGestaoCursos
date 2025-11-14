using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAlunoRepository : IRepository<Aluno>
    {
        Task<Aluno> GetByEmailAsync(string email);
        Task<Aluno> GetByCpfAsync(string cpf);
        Task<Aluno> GetByRegistroAcademicoAsync(string registroAcademico);
        Task<bool> VerificarEmailExistenteAsync(string email, int? excludeAlunoId = null);
        Task<bool> VerificarCpfExistenteAsync(string cpf, int? excludeAlunoId = null);
    }
}