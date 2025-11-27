using Application.Models;
using MediatR;


namespace Application.Queries
{
    public class GetMatriculasPorAlunoQuery : IRequest<List<MatriculaDto>>
    {
        public int Id { get; set; }
        public GetMatriculasPorAlunoQuery(int id)
        {
            Id = id;
        }
    }
}
