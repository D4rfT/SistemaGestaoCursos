using Application.Models;
using MediatR;

namespace Application.Queries
{
    public class GetAllCursosQuery : IRequest<List<CursoDto>>
    {
        public GetAllCursosQuery()
        {

        }
    }
}