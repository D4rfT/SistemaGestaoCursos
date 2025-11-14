using Application.Models;
using MediatR;


namespace Application.Queries
{
    public class GetAlunoByIdQuery : IRequest<AlunoDto>
    {
        public int Id { get; set; }

        public GetAlunoByIdQuery(int id)
        {
            Id = id;
        }
    }
}
