using Application.Models;
using MediatR;


namespace Application.Queries
{
    public class GetMatriculasPorCursoQuery : IRequest<List<MatriculaDto>>
    {
        public int Id { get; set; }
        public GetMatriculasPorCursoQuery(int id)
        {
            Id = id;
        }
    }
}
