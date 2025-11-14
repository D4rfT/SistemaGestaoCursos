using Application.Models;
using MediatR;


namespace Application.Queries
{
    public class GetAllAlunosQuery : IRequest<List<AlunoDto>>
    {
        public GetAllAlunosQuery() 
        {

        }
    }
}
