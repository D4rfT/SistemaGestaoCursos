using Application.Models;
using MediatR;

namespace Application.Queries
{
    public class GetCursoByIdQuery : IRequest<CursoDto>
    {
        public int Id { get; set; }

        public GetCursoByIdQuery(int id)
        {
            Id = id;
        }
    }
}