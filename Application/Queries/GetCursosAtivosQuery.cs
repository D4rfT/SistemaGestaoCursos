using Application.Models;
using MediatR;

namespace Application.Queries
{
    public class GetCursosAtivosQuery : IRequest<List<CursoDto>>
    {
        public GetCursosAtivosQuery()
        {

        }
    }
}
