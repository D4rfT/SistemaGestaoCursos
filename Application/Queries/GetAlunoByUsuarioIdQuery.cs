using Application.Models;
using MediatR;

namespace Application.Queries
{
    public class GetAlunoByUsuarioIdQuery : IRequest<AlunoDto>
    {
        public int UsuarioId { get; }

        public GetAlunoByUsuarioIdQuery(int usuarioId)
        {
            UsuarioId = usuarioId;
        }
    }
}