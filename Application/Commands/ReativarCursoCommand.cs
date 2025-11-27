using MediatR;

namespace Application.Commands
{
    public class ReativarCursoCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public ReativarCursoCommand(int id)
        {
            Id = id;
        }
    }
}