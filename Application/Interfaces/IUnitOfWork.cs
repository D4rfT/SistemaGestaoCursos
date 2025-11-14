namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICursoRepository Cursos { get; }
        IAlunoRepository Alunos { get; }
        IMatriculaRepository Matriculas { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}