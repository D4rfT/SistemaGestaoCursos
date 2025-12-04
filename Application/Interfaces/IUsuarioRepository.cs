using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}