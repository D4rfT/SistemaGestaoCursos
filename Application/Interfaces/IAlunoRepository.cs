using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAlunoRepository : IRepository<Aluno>
    {
        Task<Aluno> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<Aluno> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);

        Task<Aluno> GetByRegistroAcademicoAsync(string registroAcademico, CancellationToken cancellationToken = default);

        Task<bool> VerificarEmailExistenteAsync(string email, int? excludeAlunoId = null, CancellationToken cancellationToken = default);

        Task<bool> VerificarCpfExistenteAsync(string cpf, int? excludeAlunoId = null, CancellationToken cancellationToken = default);
    }
}