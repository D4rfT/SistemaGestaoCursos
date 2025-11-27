using Application.Models;
using MediatR;

namespace Application.Queries
{
    public class GetMatriculaByIdQuery : IRequest<MatriculaDto>
    {
        public int Id { get; set; }

        public GetMatriculaByIdQuery (int id)
        {
            Id = id;
        }
    }
}
