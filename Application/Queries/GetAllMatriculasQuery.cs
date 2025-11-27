using Application.Models;
using MediatR;

namespace Application.Queries
{
    public class GetAllMatriculasQuery : IRequest<List<MatriculaDto>>
    {
        public GetAllMatriculasQuery()
        {

        }
    }
}
